using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Responses;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Responses
{
    public class ProducesTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_produces_is_defined_on_all_operations_then_it_is_added_to_the_document()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(TextProducesController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "TextProduces_ProducesOnOperation").Operation;

            Assert.Contains("text/html", document.Produces);
            Assert.Contains("text/html", operation.ActualProduces);
            Assert.Null(operation.Produces);
        }
        
        [Fact]
        public async Task When_operation_produces_is_different_in_several_controllers_then_they_are_added_to_the_operation()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(TextProducesController), typeof(JsonProducesController));
            var json = document.ToJson();

            // Assert
            const string expectedTextContentType = "text/html";
            const string expectedJsonJsonType = "application/json";

            var textOperation = document.Operations.First(o => o.Operation.OperationId == "TextProduces_ProducesOnOperation").Operation;
            var jsonOperation = document.Operations.First(o => o.Operation.OperationId == "JsonProduces_ProducesOnOperation").Operation;

            Assert.DoesNotContain(expectedTextContentType, document.Produces);
            Assert.DoesNotContain(expectedJsonJsonType, document.Produces);

            Assert.Contains(expectedTextContentType, textOperation.Produces);
            Assert.Contains(expectedTextContentType, textOperation.ActualProduces);

            Assert.Contains(expectedJsonJsonType, jsonOperation.Produces);
            Assert.Contains(expectedJsonJsonType, jsonOperation.ActualProduces);
        }
    }
}
