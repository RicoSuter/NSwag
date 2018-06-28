using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.Processors;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class ApiVersionProcessorWithAspNetCoreTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_generating_v1_then_only_v1_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "1" };

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues");
            Assert.Equal(4, operations.Count());
            Assert.True(operations.All(o => o.Path.Contains("/v1/")));
        }

        [Fact]
        public async Task When_generating_v2_then_only_v2_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "2" };

            // Act
            var document = await GenerateDocumentAsync(settings);

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues");
            Assert.Equal(2, operations.Count());
            Assert.True(operations.All(o => o.Path.Contains("/v2/")));
        }
    }
}
