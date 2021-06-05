using System.Linq;
using Xunit;

namespace NSwag.Core.Tests
{
    public class OperationIdTests
    {
        [Fact]
        public void When_generating_operation_ids_then_all_are_set()
        {
            //// Arrange
            var document = new OpenApiDocument();
            document.Paths["path"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Get,
                    new OpenApiOperation { }
                },
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation { }
                }
            };

            //// Act
            document.GenerateOperationIds();

            //// Assert
            Assert.True(document.Operations.GroupBy(o => o.Operation.OperationId).All(g => g.Count() == 1));
        }

        [Fact]
        public void When_operation_ids_are_not_unique_then_method_names_are_used()
        {
            //// Arrange
            var document = new OpenApiDocument();
            document.Paths["path1"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Get,
                    new OpenApiOperation { OperationId = "foo" }
                },
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation { OperationId = "foo" }
                }
            };

            document.Paths["path2"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation { OperationId = "foo" }
                }
            };

            //// Act
            document.GenerateOperationIds();

            var actual = document.Operations.ToList();
            //// Assert
            Assert.Equal("get-foo", actual[0].Operation.OperationId);
            Assert.Equal("post-foo", actual[1].Operation.OperationId);
            Assert.Equal("post-foo2", actual[2].Operation.OperationId);
        }
    }
}
