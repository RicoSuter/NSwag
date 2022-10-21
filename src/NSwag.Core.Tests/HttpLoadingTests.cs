using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests
{
    public class HttpLoadingTests
    {
        [Fact]
        public async Task When_Swagger_is_loaded_from_url_then_it_works()
        {
            // Arrange


            // Act
            var document = await OpenApiDocument.FromUrlAsync("http://petstore.swagger.io/v2/swagger.json");

            // Assert
            Assert.NotNull(document);
        }

        [Fact]
        public async Task When_Swagger_is_loaded_from_url_schematype_is_Swagger2()
        {
            // Arrange


            // Act
            var document = await OpenApiDocument.FromUrlAsync("http://petstore.swagger.io/v2/swagger.json");

            // Assert
            Assert.True(document.SchemaType == NJsonSchema.SchemaType.Swagger2);
        }

        [Fact]
        public async Task When_openapi_is_loaded_without_scopes_it_should_deserialize()
        {
            // Arrange


            // Act
            var document = await OpenApiDocument.FromUrlAsync("https://raw.githubusercontent.com/microsoft/commercial-marketplace-openapi/main/Microsoft.Marketplace.SaaS/2018-08-31/saasapi.v2.json");

            // Assert
            Assert.True(document.SchemaType == NJsonSchema.SchemaType.OpenApi3);
        }

        // TODO: Reenable test
        [Fact(Skip = "TODO")]
        public async Task When_OpenApi_is_loaded_from_url_schematype_is_OpenApi3()
        {
            // Arrange


            // Act
            var document = await OpenApiDocument.FromUrlAsync("https://api.percipio.com/common/swagger.json");

            // Assert
            Assert.True(document.SchemaType == NJsonSchema.SchemaType.OpenApi3);
        }
    }
}