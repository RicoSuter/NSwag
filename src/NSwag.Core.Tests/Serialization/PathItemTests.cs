using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class PathItemTests
    {
        [Fact]
        public async Task When_file_contains_path_reference_to_another_file_it_is_loaded()
        {
            var document = await OpenApiDocument.FromFileAsync("TestFiles/path-reference.json");

            Assert.NotNull(document);
        }

        [Fact]
        public async Task When_file_contains_schema_reference_to_another_file_it_is_loaded()
        {
            var document = await OpenApiDocument.FromFileAsync("TestFiles/schema-reference.json");

            Assert.NotNull(document);
            Assert.Equal("Referenced test object.", document.Paths.First().Value.Values.First().Responses.First().Value.Content.First().Value.Schema.Description);
        }

        [Fact]
        public async Task When_file_contains_response_reference_to_another_file_it_is_loaded()
        {
            var document = await OpenApiDocument.FromFileAsync("TestFiles/response-reference.json");

            Assert.NotNull(document);
        }

        [Fact]
        public async Task When_file_contains_parameter_reference_to_another_file_it_is_loaded()
        {
            var document = await OpenApiDocument.FromFileAsync("TestFiles/parameter-reference.json");

            Assert.NotNull(document);
        }
    }
}
