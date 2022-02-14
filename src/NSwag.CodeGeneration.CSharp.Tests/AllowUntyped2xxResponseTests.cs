using NSwag.CodeGeneration.OperationNameGenerators;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class AllowUntyped2xxResponseTests
    {
        [Fact]
        public async Task TestAllowUntyped2xxResponseNotSet()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/definitions/{definitionId}/elements"": {
      ""get"": {
        ""operationId"": ""elements_LIST_1"",
        ""requestBody"": {
          ""content"": {
            ""*/*"": {
              ""schema"": {
                ""type"": ""integer"",
                ""format"": ""int64""
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""content"": {
                ""text/plain"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                },
              ""application/json"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                },
              ""text/json"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                }
            }
          },
          ""204"": {
            ""description"": ""Success 204"",
        }
    }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(swagger);
            //untyped
            // Act
            var codeGen = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings()
            {
                GenerateClientInterfaces = true,
                AllowUntyped2xxResponse = false,
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator()
            });

            var code = codeGen.GenerateFile();

            // Assert
            Assert.DoesNotContain("return default(string)", code);
            Assert.Contains(@"throw new ApiException(""Success 204"", status_, responseText_, headers_, null);", code);
        }

        [Theory]
        [InlineData(true, "return new SwaggerResponse<string>(status_, headers_, default(string));")]
        [InlineData(false, "return default(string);")]
        public async Task TestAllowUntyped2xxResponseIsTrueWrappedOrNot(bool wrapResponse, string expectedResult)
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/definitions/{definitionId}/elements"": {
      ""get"": {
        ""operationId"": ""elements_LIST_1"",
        ""requestBody"": {
          ""content"": {
            ""*/*"": {
              ""schema"": {
                ""type"": ""integer"",
                ""format"": ""int64""
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""content"": {
                ""text/plain"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                },
              ""application/json"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                },
              ""text/json"": {
                    ""schema"": {
                        ""type"": ""string""
                    }
                }
            }
          },
          ""204"": {
            ""description"": ""Success 204"",
        }
    }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(swagger);
            //untyped
            // Act
            var codeGen = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings()
            {
                GenerateClientInterfaces = true,
                AllowUntyped2xxResponse = true,
                WrapResponses = wrapResponse,
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator()
            });

            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains(expectedResult, code);
            Assert.DoesNotContain(@"throw new ApiException(""Success 204"", status_, responseText_, headers_, null);", code);
        }
    }
}