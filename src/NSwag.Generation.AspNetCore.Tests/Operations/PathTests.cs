using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Operations
{
    public class PathTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_route_is_empty_then_path_is_slash()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(EmptyPathController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "EmptyPath_Get");

            Assert.Equal("/", operation.Path);
        }

        [Fact]
        public async Task When_route_is_not_empty_then_path_starts_with_slash()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(BodyParametersController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "BodyParameters_RequiredPrimitive");

            Assert.StartsWith("/", operation.Path);
        }
    }
}
