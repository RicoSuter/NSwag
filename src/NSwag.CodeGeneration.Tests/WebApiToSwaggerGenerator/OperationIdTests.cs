using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator
{
    [TestClass]
    public class OperationIdTests
    {
        public class OperationIdController : ApiController
        {
            [Route("Foo")]
            [SwaggerOperation("MyFoo")]
            public void Foo()
            {
                
            }

            [Route("Bar")]
            public void Bar()
            {
                
            }
        }

        [TestMethod]
        public void When_SwaggerOperation_attribute_is_available_then_operation_id_is_correct()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<OperationIdController>();

            //// Assert
            Assert.AreEqual("MyFoo", service.Operations.First(o => o.Path == "/Foo").Operation.OperationId);
            Assert.AreEqual("OperationId_Bar", service.Operations.First(o => o.Path == "/Bar").Operation.OperationId);
        }
    }
}
