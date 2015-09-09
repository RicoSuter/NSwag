using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Tests.Specification
{
    [TestClass]
    public class OperationIdTests
    {
        [TestMethod]
        public void When_generating_operation_id()
        {
            //// Arrange
            var service = new SwaggerService();
            service.Paths["path"] = new SwaggerOperations
            {
                {
                    SwaggerOperationMethod.get,
                    new SwaggerOperation { }
                },
                {
                    SwaggerOperationMethod.post,
                    new SwaggerOperation { }
                }
            };

            //// Act
            service.GenerateOperationIds();

            //// Assert
            Assert.IsTrue(service.Operations.GroupBy(o => o.Operation.OperationId).All(g => g.Count() == 1));
        }
    }
}
