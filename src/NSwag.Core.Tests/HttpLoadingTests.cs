using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests
{
    public class HttpLoadingTests
    {
        [Fact]
        public async Task When_Swagger_is_loaded_from_url_then_it_works()
        {
            //// Arrange


            //// Act
            var document = await SwaggerDocument.FromUrlAsync("http://petstore.swagger.io/v2/swagger.json");

            //// Assert
            Assert.NotNull(document);
        }
    }
}