using NSwag.CodeGeneration.CSharp;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class InheritanceTests
    {
        [Fact]
        public async Task When_generating_csharp_with_any_inheritance_then_inheritance_is_generated()
        {
            // Arrange
            var json = @"{
    ""openapi"": ""3.0.0"",
    ""components"": {
        ""schemas"": {
            ""SessionStateResent"": {
                ""allOf"": [
                    {
                        ""$ref"": ""#/components/schemas/SportsbookEventBody""
                    },
                    {
                        ""type"": ""object"",
                        ""additionalProperties"": false
                    }
                ]
            },
            ""SportsbookEventBody"": {
                ""type"": ""object"",
                ""additionalProperties"": false
            }
        }
    }
}";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings();
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("class SessionStateResent : SportsbookEventBody", code);
        }
    }
}
