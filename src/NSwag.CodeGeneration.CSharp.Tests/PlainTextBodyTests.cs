using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class PlainTextBodyTests
    {
        [Fact]
        public async Task When_request_contains_plain_text_string_body_then_ReadObjectResponseAsync_is_generated()
        {
            // Arrange
            var json = @"{
  ""swagger"":  ""2.0"",
  ""paths"": {
    ""/api/get-ienumerable"": {
    ""put"":  {
      ""tags"":  [
        ""BuildType""
      ],
      ""summary"":  ""Update value of build parameter."",
      ""description"":  ""plain text body"",
      ""operationId"":  ""updateBuildParameterValueOfBuildType"",
      ""consumes"":  [
        ""text/plain""
      ],
      ""produces"":  [
        ""text/plain""
      ],
      ""parameters"":  [
          {
            ""name"":  ""name"",
            ""in"":  ""path"",
            ""required"":  true,
            ""type"":  ""string""
          },
          {
            ""in"":  ""body"",
            ""name"":  ""body"",
              ""required"":  false,
              ""schema"":  {
              ""type"":  ""string""
            }
          },
          {
            ""name"":  ""btLocator"",
            ""in"":  ""path"",
            ""required"":  true,
            ""type"":  ""string"",
            ""format"":  ""BuildTypeLocator""
          }
      ],
      ""responses"":  {
        ""200"":  {
          ""description"":  ""successful operation"",
          ""examples"":  {
            ""text/plain"":  ""plan text""
          },
          ""schema"":  {
            ""type"":  ""string""
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
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
