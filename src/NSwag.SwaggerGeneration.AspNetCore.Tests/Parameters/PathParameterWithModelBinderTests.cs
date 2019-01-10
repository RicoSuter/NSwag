using System.Linq;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Parameters
{
    public class PathParameterWithModelBinderTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_model_binder_parameter_is_used_on_path_parameter_then_parameter_kind_is_path()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings { RequireParametersWithoutDefault = true };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(PathParameterWithModelBinderController));

            // Assert
            var kind = document.Operations.First().Operation.Parameters.First().Kind;
            Assert.Equal(SwaggerParameterKind.Path, kind);
        }
    }
}
