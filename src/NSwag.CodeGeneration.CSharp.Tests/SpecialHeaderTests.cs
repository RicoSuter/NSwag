using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NJsonSchema;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class SpecialHeaderTests
    {
        private string openapi_schema = @"{
  ""x-generator"": ""NSwag v13.0.6.0 (NJsonSchema v10.0.23.0 (Newtonsoft.Json v12.0.0.0))"",
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""title"": ""Apimundo API"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/api/endpoints/{endpointId}/environments/{environmentId}/documents/openapi/upload_segmenets"": {
      ""post"": {
        ""tags"": [
          ""Endpoints""
        ],
        ""summary"": ""Upload file segments."",
        ""operationId"": ""Endpoints_TestContentDispositionHeader"",
        ""parameters"": [{
                ""name"": ""Content-Disposition"",
                ""in"": ""header"",
                ""description"": ""Name of attachment"",
                ""required"": true,
                ""schema"": {
                  ""type"": ""string""
                },
                ""example"": ""attachment; filename='file.ext'""
            },
            {
                ""name"": ""X-Session-ID"",
                ""in"": ""header"",
                ""description"": ""Session ID"",
                ""required"": true,
                ""schema"": {
                ""type"": ""string""
                },
                ""example"": 123456789
            },
            {
                ""name"": ""X-Content-Range"",
                ""in"": ""header"",
                ""description"": ""Byte Range of uploaded segement (segmentStart-segmentEnd/totalFileSize)"",
                ""required"": true,
                ""schema"": {
                    ""type"": ""string""
                },
                ""example"": ""bytes 0-40000/340000""
            }],
        ""requestBody"": {
            ""description"": ""Segment of the file, corresponding to the range that was specified in `X-Content-Range` headers "",
            ""content"": {
                ""application/octet-stream"": {
                ""schema"": {
                    ""type"": ""string"",
                    ""format"": ""binary""
                }
                }
            }
        },
        ""responses"": {
          ""200"": {
            ""description"": ""The comparison result."",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": { }
  }
}";

        [Fact]
        public async Task When_openapi3_contains_content_disposition_header_client()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(openapi_schema, null, SchemaType.OpenApi3, null);

            // Act
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            // Test required
            Assert.Contains("throw new System.ArgumentNullException(\"content_Disposition\");", code);
            Assert.Contains("request_.Content.Headers.ContentDisposition = content_Disposition;", code);
        }

        [Fact]
        public async Task When_openapi3_contains_content_disposition_header_controller()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(openapi_schema, null, SchemaType.OpenApi3, null);

            // Act
            var codeGenerator = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings()
            {
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            // Test required
            Assert.Contains("TestContentDispositionHeaderAsync(System.Net.Http.Headers.ContentDispositionHeaderValue content_Disposition", code);
            Assert.Contains("TestContentDispositionHeader([Microsoft.AspNetCore.Mvc.FromHeader(Name = \"X-Session-ID\")] string x_Session_ID,", code);
            Assert.Contains("_implementation.TestContentDispositionHeaderAsync(Request.Headers.ContentDisposition,", code);

            Assert.Single(Regex.Matches(code, "\"content_Disposition\""));
        }

        [Fact]
        public async Task When_openapi3_contains_content_disposition_header_abstract_controller()
        {
            // Arrange
            var document = await OpenApiDocument.FromJsonAsync(openapi_schema, null, SchemaType.OpenApi3, null);

            // Act
            var codeGenerator = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings()
            {
                ControllerStyle = Models.CSharpControllerStyle.Abstract
            });
            var code = codeGenerator.GenerateFile();

            // Assert
            // Test required
            Assert.Contains("public abstract System.Threading.Tasks.Task<string> TestContentDispositionHeader([Microsoft.AspNetCore.Mvc.FromHeader(Name = \"X-Session-ID\")]", code);
        }
    }
}
