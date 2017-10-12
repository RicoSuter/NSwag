using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    [TestClass]
    public class AspNetCoreToSwaggerGenerationTests
    {
        [TestMethod]
        public async Task When_generating_swagger_all_apidescriptions_are_discovered()
        {
            //// Arrange
            var generator = new AspNetCoreToSwaggerGenerator(new AspNetCoreToSwaggerGeneratorSettings());
            var apiDescriptions = new[]
            {
                new ApiDescription
                {
                    ActionDescriptor = new ControllerActionDescriptor
                    {
                        MethodInfo = typeof(TestController).GetMethod(nameof(TestController.FindModel)),
                    },
                    HttpMethod = "Get",
                    RelativePath = "/test",
                    ParameterDescriptions =
                    {
                        new ApiParameterDescription
                        {
                            Name = "id",
                            Source = BindingSource.Path,
                            Type = typeof(int),
                        },
                    },
                    SupportedResponseTypes =
                    {
                        new ApiResponseType
                        {
                            Type = typeof(TestModel),
                            StatusCode = 200,
                        },
                        new ApiResponseType
                        {
                            Type = typeof(ProblemDetails),
                            StatusCode = 404,
                        },
                        new ApiResponseType2_1
                        {
                            Type = typeof(ProblemDetails),
                            IsDefaultResponse = true,
                        },
                    },
                },
            };

            //// Act
            var document = await generator.GenerateAsync(apiDescriptions);
            var swaggerSpecification = document.ToJson();

            //// Assert
            Assert.AreEqual(1, document.Operations.Count());

            var operationDescription = document.Operations.ElementAt(0);
            Assert.AreEqual("/test", operationDescription.Path);
            Assert.AreEqual(SwaggerOperationMethod.Get, operationDescription.Method);

            var operation = operationDescription.Operation;
            Assert.AreEqual("Test_FindModel", operation.OperationId);

            Assert.AreEqual(1, operation.Parameters.Count);
            var parameter = operation.Parameters[0];
            Assert.AreEqual("id", parameter.Name);
            Assert.AreEqual(SwaggerParameterKind.Path, parameter.Kind);
            Assert.IsTrue(parameter.IsRequired);
            Assert.AreEqual(NJsonSchema.JsonObjectType.Integer, parameter.Type);

            Assert.AreEqual(3, operation.Responses.Count());
            var response = operation.Responses["200"];
            Assert.AreEqual("#/definitions/TestModel", response.Schema.ReferencePath);

            response = operation.Responses["404"];
            Assert.AreEqual("#/definitions/ProblemDetails", response.Schema.ReferencePath);

            response = operation.Responses["default"];
            Assert.AreEqual("#/definitions/ProblemDetails", response.Schema.ReferencePath);
        }

        private class TestController : ControllerBase
        {
            public Task<TestModel> FindModel([FromRoute] int id) => null;
        }

        private class TestModel { }

        private class ProblemDetails { }

        private class ApiResponseType2_1 : ApiResponseType
        {
            public bool IsDefaultResponse { get; set; }
        }
    }
}
