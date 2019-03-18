using NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Requests;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Requests
{
    public class ProducesTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_produces_is_defined_on_all_operations_then_it_is_added_to_the_document()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ProducesController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "Produces_ProducesOnOperation").Operation;

            Assert.Contains("text/html", document.Produces);
            Assert.Contains("text/html", operation.ActualProduces);
            Assert.Null(operation.Produces);
        }
    }
}
