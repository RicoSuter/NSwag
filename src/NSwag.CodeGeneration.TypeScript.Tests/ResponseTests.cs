using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class ResponseTests
    {
        [Fact]
        public async Task When_multiple_2xx_responses_of_same_type_they_are_merged_correctly()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.0"",
  ""paths"": {
    ""/v1/exceptions/get"": {
      ""post"": {
        ""operationId"": ""Exceptions_GetException"",
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/BusinessException""
                }
              }
            }
          },
          ""201"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/BusinessException""
                }
              }
            }
          },
          ""204"": {
            ""description"":""No Content""
          },
          ""404"": {
            ""description"":""Not Found""
          }
        }
      }
    }
  }, 
  ""components"": {
    ""schemas"": {
      ""BusinessException"": {
        ""type"": ""object"",
        ""additionalProperties"": false
      }
    }
  }
}";
            // Act
            var document = await OpenApiDocument.FromJsonAsync(json);
            var settings = new TypeScriptClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new TypeScriptClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("if (status === 200 || status === 201)", code);
            Assert.Contains("else if (status === 204)", code);
            Assert.Contains("else if (status === 404)", code);
            Assert.Contains("else if (status !== 200 && status !== 204)", code);
        }
    }
}