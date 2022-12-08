using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Generation.WebApi;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class PlainResponseTests
    {
        [Fact]
        public async Task When_openapi3_reponse_contains_plain_text_string_array_then_ReadObjectResponseAsync_is_generated()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/api/get-ienumerable"": {
      ""get"": {
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""content"": {
              ""text/plain"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""type"": ""string"",
                    ""nullable"": true
                  },
                  ""nullable"": true
                }
              },
              ""application/json"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""type"": ""string"",
                    ""nullable"": true
                  },
                  ""nullable"": true
                }
              },
              ""text/json"": {
                ""schema"": {
                  ""type"": ""array"",
                  ""items"": {
                    ""type"": ""string"",
                    ""nullable"": true
                  },
                  ""nullable"": true
                }
              }
            }
          }
        }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains($"public virtual async System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> GetIenumerableAsync(", code);
            Assert.Contains("await ReadObjectResponseAsync<System.Collections.Generic.ICollection<string>>(", code);
        }

        [Fact]
        public async Task When_openapi3_reponse_contains_plain_text_then_Convert_is_generated()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/instances/text"": {
      ""get"": {
        ""description"": ""sample"",
        ""operationId"": ""plain"",
        ""parameters"": [],
        ""responses"": {
          ""200"": {
            ""description"": ""plain string return"",
            ""content"": {
              ""text/plain"": {
                ""schema"": {
                  ""type"": ""string"",
                }
              }
            }
          }
        }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains($"public virtual async System.Threading.Tasks.Task<string> PlainAsync(", code);
            Assert.Contains("(string)System.Convert.ChangeType(responseData_, typeof(string));", code);
        }

        [Fact]
        public async Task When_swagger_reponse_contains_plain_text_string_array_then_ReadObjectResponseAsync_is_generated()
        {
            // Arrange
            var swagger = @"{
  ""swagger"": ""2.0"",
  ""paths"": {
    ""/api/get-ienumerable"": {
      ""get"": {
        ""produces"": [
          ""text/plain"",
          ""application/json"",
          ""text/json""
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            }
          }
        }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains($"public virtual System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> GetIenumerableAsync(", code);
            Assert.Contains("await ReadObjectResponseAsync<System.Collections.Generic.ICollection<string>>(", code);
        }

        [Fact]
        public async Task When_swagger_reponse_contains_plain_text_then_Convert_is_generated()
        {
            // Arrange
            var swagger = @"{
  ""swagger"" : ""2.0"",
  ""paths"" : {
    ""/instances/text"": {
      ""get"": {
        ""description"": ""sample"",
        ""operationId"": ""plain"",
        ""produces"": [ ""text/plain"" ],
        ""responses"": {
          ""200"": {
            ""description"": ""plain string return"",
            ""schema"": {
                ""type"": ""string"",
              }
          }
        }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains($"public virtual async System.Threading.Tasks.Task<string> PlainAsync(", code);
        }
    }
}
