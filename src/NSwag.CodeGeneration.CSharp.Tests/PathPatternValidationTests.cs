using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using System.IO;


namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class PathPatternValidationTests
    {
        private static string GenerateSpec(bool withPattern)
        {
            return $@"{{
  ""openapi"": ""3.0.1"",
  ""info"": {{
    ""title"": ""NSwager Test Server API"",
    ""description"": ""An api used to test NSwager."",
    ""version"": ""2.0""
  }},
  ""paths"": {{
    ""/api/v2/test/{{PathVariable}}/ping"": {{
      ""get"": {{
        ""tags"": [
          ""TestControllerVersionTwo""
        ],
        ""summary"": ""Used to ping a valid user name."",
        ""operationId"": ""PingpathVariableV2"",
        ""parameters"": [
          {{
            ""name"": ""pathVariable"",
            ""in"": ""path"",
            ""description"": ""A System.String"",
            ""required"": true,
            ""schema"": {{
              {(withPattern ? @"""pattern"": ""^[a-zA-Z0-9_]+$"", " : "")}
              ""type"": ""string""
            }}
          }}
        ],
        ""responses"": {{
          ""200"": {{
            ""description"": ""With the user name"",
            ""content"": {{
              ""application/json"": {{
                ""schema"": {{
                  ""$ref"": ""#/components/schemas/PathVariableDTO""
                }}
              }}
            }}
          }}
        }}
      }}
    }}
  }},
  ""components"": {{
    ""schemas"": {{
      ""PathVariableDTO"": {{
        ""required"": [
          ""pathVariable""
        ],
        ""type"": ""object"",
        ""properties"": {{
          ""pathVariable"": {{
            ""type"": ""string"",
            ""description"": ""The value of the path variable"",
            ""nullable"": true
          }}
        }},
        ""additionalProperties"": false,
        ""description"": ""A DTO containing a path variable""
      }}
    }}
  }}
}}";
        }

        /// <summary>
        /// This string if statement is exactly the same as the if statement in <see cref="ValidatePatternValueMock"/> method.
        /// </summary>
        private string generatedCode =
@"if (!System.Text.RegularExpressions.Regex.IsMatch(pathVariable, ""^[a-zA-Z0-9_]+$""))
    throw new System.ArgumentException(""Parameter 'pathVariable' does not match the required pattern '^[a-zA-Z0-9_]+$'."");";

        /// <summary>
        /// Used to mock execution of the generated code <see cref="generatedCode"/>
        /// </summary>
        /// <param name="pathVariable">The value of the path variable.</param>
        /// <param name="regexPattern">A regular expression use to validate <see cref="pathVariable"/>.</param>
        /// <exception cref="System.ArgumentException">Thrown if <paramref name="pathVariable"/>
        /// does not match the <paramref name="regexPattern"/>.</exception>
        private static void ValidatePathPatternMock(string pathVariable, string regexPattern)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(pathVariable, regexPattern))
                throw new System.ArgumentException(
                    $"Parameter 'pathVariable' does not match the required pattern '{regexPattern}'."
                );
        }

        [Fact]
        public async Task When_path_parameter_have_pattern_field()
        {
            // Arrange
            var seetings = new CSharpClientGeneratorSettings();
            var document = await OpenApiDocument.FromJsonAsync(GenerateSpec(withPattern: true));
            var generator = new CSharpClientGenerator(document, seetings);

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains(NormalizeWhitespace(generatedCode), NormalizeWhitespace(code));
        }

        [Fact]
        public async Task When_path_parameter_not_have_pattern_field()
        {
            // Arrange
            var seetings = new CSharpClientGeneratorSettings();
            var document = await OpenApiDocument.FromJsonAsync(GenerateSpec(withPattern: false));
            var generator = new CSharpClientGenerator(document, seetings);

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.DoesNotContain(generatedCode, code);
        }

        [Theory]
        [InlineData("MockValue123", "^[a-zA-Z0-9_]+$")] // Alphanumeric and underscores
        [InlineData("MockValue-123", "^[a-zA-Z0-9_-]+$")] // Alphanumeric, underscores, and dashes
        [InlineData("Mock.Value.123", "^[a-zA-Z0-9._]+$")] // Alphanumeric, dots, and underscores
        [InlineData("123456", "^\\d+$")] // Digits only
        public void ValidatePathVariable_ValidPathVariables_DoesNotThrow(
            string pathVariable,
            string regexPattern
        )
        {
            // Arrange & Act & Assert
            var exception = Record.Exception(
                () => ValidatePathPatternMock(pathVariable, regexPattern)
            );
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Mock@123", "^[a-zA-Z0-9_]+$")] // Alphanumeric and underscores
        [InlineData("Mock Value", "^[a-zA-Z0-9_-]+$")] // Alphanumeric, underscores, and dashes
        [InlineData("Mock!Value", "^[a-zA-Z0-9._]+$")] // Alphanumeric, dots, and underscores
        [InlineData("MockValue123", "^\\d+$")] // Digits only
        public void ValidatePathVariable_InvalidPathVariables_ThrowsArgumentException(
            string pathVariable,
            string regexPattern
        )
        {
            // Arrange & Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => ValidatePathPatternMock(pathVariable, regexPattern)
            );
            Assert.Contains(
                $"Parameter 'pathVariable' does not match the required pattern",
                exception.Message
            );
        }

        private static string NormalizeWhitespace(string input)
        {
            char[] separators = ['\r', '\n', '\t', ' '];
            return string.Join(
                " ",
                input.Split(separators, StringSplitOptions.RemoveEmptyEntries)
            );
        }
    }
}