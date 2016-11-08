using System.Linq;
using System.Net;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class ResponseAttributeTests
    {
        public class ResponseAttributeTestController : ApiController
        {
            [Route("Foo")]
            [ResponseType(HttpStatusCode.Conflict, typeof(string))]
            public void Foo()
            {

            }

            [Route("Bar")]
            [SwaggerResponse(HttpStatusCode.Created, typeof(int))]
            public void Bar()
            {

            }
        }

        [TestMethod]
        public void When_operation_has_ResponseTypeAttribute_then_it_is_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<ResponseAttributeTestController>();

            //// Assert
            var fooOperation = document.Operations.Single(o => o.Operation.OperationId == "ResponseAttributeTest_Foo");
            Assert.AreEqual("409", fooOperation.Operation.Responses.First().Key);
            Assert.AreEqual(JsonObjectType.String, fooOperation.Operation.Responses.First().Value.Schema.Type);
        }

        [TestMethod]
        public void When_operation_has_SwaggerResponseAttribute_then_it_is_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<ResponseAttributeTestController>();

            //// Assert
            var barOperation = document.Operations.Single(o => o.Operation.OperationId == "ResponseAttributeTest_Bar");
            Assert.AreEqual("201", barOperation.Operation.Responses.First().Key);
            Assert.AreEqual(JsonObjectType.Integer, barOperation.Operation.Responses.First().Value.Schema.Type);
        }
    }
}
