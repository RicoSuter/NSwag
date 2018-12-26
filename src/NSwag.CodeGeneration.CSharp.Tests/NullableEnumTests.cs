using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class NullableEnumTests
    {
        [Fact]
        public async Task When_enum_is_not_required_then_property_is_nullable()
        {
            //// Arrange
            const string swagger = @"
{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": ""Swagger Petstore"",
    ""license"": {
      ""name"": ""MIT""
    },
    ""version"": ""1.0.0""
  },
  ""host"": ""petstore.swagger.io"",
  ""basePath"": ""/v1"",
  ""schemes"": [
    ""http""
  ],
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""definitions"": {
    ""Sex"": {
      ""type"": ""string"",
      ""enum"": [
        ""male"",
        ""female""
      ]
    },
    ""Pet"": {
      ""required"": [
        ""id"",
        ""name""
      ],
      ""properties"": {
        ""id"": {
          ""type"": ""integer"",
          ""format"": ""int64""
        },
        ""name"": {
          ""type"": ""string""
        },
        ""sex"": {
          ""$ref"": ""#/definitions/Sex""
        }
      }
    }
  }
}";
            var document = await SwaggerDocument.FromJsonAsync(swagger);

            //// Act
            var settings = new SwaggerToCSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new SwaggerToCSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            //// Assert
            Assert.Contains(@"public Sex? Sex", code);
        }
    }
}