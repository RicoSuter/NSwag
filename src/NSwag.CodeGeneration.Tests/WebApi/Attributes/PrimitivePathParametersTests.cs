using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class PrimitivePathParametersTests
    {
        public class TestController : ApiController
        {
            public string WithoutAttribute(string id)
            {
                return string.Empty;
            }

            public string WithFromUriAttribute([FromUri] string id)
            {
                return string.Empty;
            }

            public string WithFromBodyAttribute([FromBody] string id)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public void When_parameter_is_primitive_then_it_is_a_path_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithoutAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Path, operation.ActualParameters[0].Kind);
        }

        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromUri_then_it_is_a_path_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromUriAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Path, operation.ActualParameters[0].Kind);
        }


        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromBody_then_it_is_a_path_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromBodyAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Path, operation.ActualParameters[0].Kind); // TODO: What is correct?
        }

        [RoutePrefix("api/RoutePrefixWithPaths/{companyIdentifier:guid}")]
        public class RoutePrefixWithPathsController : ApiController
        {
            [HttpGet]
            [Route("documents")]
            //[CompanyFilterAuthorization(permission: ApplicationPermissions.Company.SDL.Read)]
            public IHttpActionResult GetDocuments(string query)
            {
                return Ok(1);
            }
        }

        [TestMethod]
        public void When_route_has_path_parameter_which_is_not_an_action_parameter_then_it_is_still_added_as_path_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}", 
                AddMissingPathParameters = true
            });

            //// Act
            var document = generator.GenerateForController<RoutePrefixWithPathsController>();
            
            //// Assert
            var operation = document.Operations.First().Operation;
            var parameter = operation.Parameters.Single(p => p.Name == "companyIdentifier");

            Assert.AreEqual(2, operation.ActualParameters.Count);
            Assert.AreEqual(SwaggerParameterKind.Path, parameter.Kind);
            Assert.AreEqual(JsonObjectType.String, parameter.Type);
            Assert.AreEqual(JsonFormatStrings.Guid, parameter.Format);
        }
    }
}