using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator.Attributes
{
    [TestClass]
    public class PrimitiveQueryParametersTests
    {
        public class TestController : ApiController
        {
            public string WithoutAttribute(string foo)
            {
                return string.Empty;
            }

            public string WithFromUriAttribute([FromUri] string foo)
            {
                return string.Empty;
            }

            public string WithFromBodyAttribute([FromBody] string foo)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public void When_parameter_is_primitive_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "WithoutAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.Parameters[0].Kind);
        }

        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromUri_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "WithFromUriAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.Parameters[0].Kind);
        }


        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromBody_then_it_is_a_body_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "WithFromBodyAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Body, operation.Parameters[0].Kind);
        }
    }
}
