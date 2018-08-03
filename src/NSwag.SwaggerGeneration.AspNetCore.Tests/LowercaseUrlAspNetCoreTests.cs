using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.AspNetCore.Tests.Web;
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

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            Assert.True(document.Operations.All(o => o.Path == o.Path.ToLowerInvariant()));
        }
    }
}