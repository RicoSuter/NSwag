using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Inheritance;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Inheritance
{
    public class InheritanceControllerTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_primitive_body_parameter_has_no_default_value_then_it_is_required()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings { SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.OpenApi3 } };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ActualController));
            var json = document.ToJson();

            // Assert
            Assert.True(document.Operations.Any());
        }
    }
}