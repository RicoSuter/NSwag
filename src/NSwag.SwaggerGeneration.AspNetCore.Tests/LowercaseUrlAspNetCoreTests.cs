using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.AspNetCore.Tests.Web;
using NSwag.SwaggerGeneration.Processors;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class LowercaseUrlAspNetCoreTests : AspNetCoreTestsBase<LowercaseUrlStartup>
    {
        [Fact]
        public async Task When_LowercaseUrls_option_is_set_then_all_paths_must_be_lowercase()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] {"1"};

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            var operations = GetControllerOperations(document, "VersionedValues").ToArray();
            Assert.True(operations.All(o => o.Path == o.Path.ToLowerInvariant()));
        }
    }
}