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

            [Route("Overload1")]
            public void Overload()
            {

            }

            [Route("Overload2")]
            public void Overload(int id)
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

        [TestMethod]
        public void When_method_has_overload_then_operation_ids_are_still_unique()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<OperationIdController>();

            //// Assert
            var allIds = service.Operations.Select(o => o.Operation.OperationId).ToArray();
            Assert.AreEqual(4, allIds.Distinct().Count());
        }
    }
}
