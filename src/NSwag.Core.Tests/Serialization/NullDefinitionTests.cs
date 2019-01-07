using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class NullDefinitionTests
    {
        [Fact]
        public async Task When_document_has_response_examples_then_it_is_serialized_in_Swagger()
        {
            //// Arrange
            var json = @"{ ""definitions"": { ""definitions"": null } }";

            //// Act
            var document = await SwaggerDocument.FromJsonAsync(json);

            //// Assert
            Assert.True(document.Definitions.Count == 0);
        }
    }
}