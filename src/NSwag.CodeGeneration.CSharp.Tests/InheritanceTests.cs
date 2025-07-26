using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp.Tests;
using NSwag.CodeGeneration.Tests;

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
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
