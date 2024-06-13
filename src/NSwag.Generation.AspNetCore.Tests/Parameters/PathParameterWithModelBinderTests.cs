using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class PathParameterWithModelBinderTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_model_binder_parameter_is_used_on_path_parameter_then_parameter_kind_is_path()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.OpenApi3 }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(PathParameterWithModelBinderController));

            // Assert
            var kind = document.Operations.First().Operation.Parameters.First().Kind;
            Assert.Equal(OpenApiParameterKind.Path, kind);
        }
    }
}
