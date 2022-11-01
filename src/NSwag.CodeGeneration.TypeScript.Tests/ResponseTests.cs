using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class ResponseTests
    {
        [Fact]
        public async Task When_response_status_code_is_a_range()
        {
            var json = @"{
              ""x-generator"": ""NSwag v13.5.0.0 (NJsonSchema v10.1.15.0 (Newtonsoft.Json v11.0.0.0))"",
              ""openapi"": ""3.0.0"",
              ""info"": {
                ""title"": ""My Title"",
                ""version"": ""1.0.0""
              },
              ""paths"": {
                ""/api/AwesomeTest/DoStuff"": {
                  ""post"": {
                    ""tags"": [
                      ""DoStuff""
                    ],
                    ""operationId"": ""AwesomeTest_DoStuff"",
                    ""requestBody"": {
                      ""content"": {
                        ""text/html"": {
                          ""schema"": {
                            ""type"": ""string""
                          }
                        }
                      }
                    },
                    ""responses"": {
                      ""200"": {
                        ""description"": """",
                        ""content"": {
                          ""text/html"": {
                            ""schema"": {
                              ""type"": ""string""
                            }
                          }
                        }
                      },
                      ""5xX"": {
                        ""description"": """",
                        ""content"": {
                          ""text/html"": {
                            ""schema"": {
                              ""type"": ""string""
                            }
                          }
                        }
                      }
                    }
                  }
                },
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            var clientSettings = new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Axios,
                TypeScriptGeneratorSettings =
                {
                  TypeScriptVersion = 1.8m
                }
            };

            var gen = new TypeScriptClientGenerator(document, clientSettings);
            var code = gen.GenerateFile();

            // Assert
            Assert.Contains("if (`${status}`.match('^5xX$'.replace(/x/gi, '\\\\d')))", code);
            Assert.Contains("if (status === 200)", code);
        }
    }
}
