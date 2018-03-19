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
            var document = new SwaggerDocument();
            document.Paths["path"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Get,
                    new SwaggerOperation { }
                },
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation { }
                }
            };

            //// Act
            document.GenerateOperationIds();

            //// Assert
            Assert.IsTrue(document.Operations.GroupBy(o => o.Operation.OperationId).All(g => g.Count() == 1));
        }
    }
}
