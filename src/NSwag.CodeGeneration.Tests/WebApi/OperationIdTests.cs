using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Annotations;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi
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
        public async Task When_SwaggerOperation_attribute_is_available_then_operation_id_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<OperationIdController>();

            //// Assert
            Assert.AreEqual("MyFoo", document.Operations.First(o => o.Path == "/Foo").Operation.OperationId);
            Assert.AreEqual("OperationId_Bar", document.Operations.First(o => o.Path == "/Bar").Operation.OperationId);
        }

        [TestMethod]
        public async Task When_method_has_overload_then_operation_ids_are_still_unique()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<OperationIdController>();

            //// Assert
            var allIds = document.Operations.Select(o => o.Operation.OperationId).ToArray();
            Assert.AreEqual(4, allIds.Distinct().Count());
        }
    }
}
