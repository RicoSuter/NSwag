using NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Requests;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Requests
{
    public class ConsumesTests : AspNetCoreTestsBase
    {
        // These test required the CustomTextInputFormatter

        [Fact]
        public async Task When_consumes_is_defined_on_all_operations_then_it_is_added_to_the_document()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ConsumesController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "Consumes_ConsumesOnOperation").Operation;

            Assert.Contains("text/html", document.Consumes);
            Assert.Contains("text/html", operation.ActualConsumes);
        }

        [Fact]
        public async Task When_consumes_is_defined_on_single_operations_then_it_is_added_to_the_operation()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ConsumesController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "Consumes_ConsumesOnOperation").Operation;

            Assert.DoesNotContain("foo/bar", document.Consumes);
            Assert.Contains("foo/bar", operation.Consumes);
            Assert.Contains("foo/bar", operation.ActualConsumes);
        }
    }
}
