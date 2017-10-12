using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class AspNetCoreToSwaggerGenerationTests
    {
        [Fact]
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
                    RelativePath = "test",
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
            var operationDescription = Assert.Single(document.Operations);
            Assert.Equal("/test", operationDescription.Path);
            Assert.Equal(SwaggerOperationMethod.Get, operationDescription.Method);

            var operation = operationDescription.Operation;
            Assert.Equal("Test_FindModel", operation.OperationId);

            var parameter = Assert.Single(operation.Parameters);
            Assert.Equal("id", parameter.Name);
            Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
            Assert.True(parameter.IsRequired);
            Assert.Equal(NJsonSchema.JsonObjectType.Integer, parameter.Type);

            Assert.Equal(3, operation.Responses.Count());
            var response = operation.Responses["200"];
            Assert.Equal("#/definitions/TestModel", response.Schema.ReferencePath);

            response = operation.Responses["404"];
            Assert.Equal("#/definitions/ProblemDetails", response.Schema.ReferencePath);

            response = operation.Responses["default"];
            Assert.Equal("#/definitions/ProblemDetails", response.Schema.ReferencePath);
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
