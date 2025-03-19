using NSwag.CodeGeneration.OperationNameGenerators;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class IdentifierTests
    {
        [Theory(DisplayName = "Ensure that generated operation names are valid C# identifiers")]
        [InlineData("test", "Test")]
        [InlineData("_test", "_test")]
        [InlineData("1test", "_1test")]
        [InlineData("1", "_1")]
        [InlineData("#1", "_1")]
        [InlineData("/path1/path2/>pathWithInvalidPrefixChar", "_path1_path2__pathWithInvalidPrefixChar")]
        public async Task When_generating_CSharp_code_ensure_valid_identifiers_are_generated(string identifier, string expectedFixedIdentifier)
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""Test"",
    ""version"": ""1.0.0""
  },
  ""paths"": {
    ""/test"": {
      ""get"": {
        ""operationId"": """ + identifier + @""",
        ""responses"": {
          ""200"": {
            ""description"": ""Success""
          }
        }
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var codeGen = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings()
            {
                UseBaseUrl = false,
                GenerateClientInterfaces = true,
                OperationNameGenerator = new SingleClientFromOperationIdOperationNameGenerator()
            });

            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains($"public virtual System.Threading.Tasks.Task {expectedFixedIdentifier}Async()", code);
        }
    }
}