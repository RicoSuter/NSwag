using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Responses
{
    public class XmlDocsTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_operation_has_SwaggerResponseAttribute_with_description_it_is_in_the_spec()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings { SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.OpenApi3 } };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(XmlDocsController));
            var json = document.ToJson();

            // Assert
            Assert.Contains("My response.", json);
            Assert.Contains("My property.", json);
        }
    }
}