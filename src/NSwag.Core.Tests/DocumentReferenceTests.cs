using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.Core.Tests
{
    public class DocumentReferenceTests
    {
        [Fact]
        public async Task When_response_is_referenced_then_it_should_be_resolved()
        {
            //// Arrange
            var json = @"
{
  ""swagger"": ""2.0"",
  ""paths"": {
    ""/resources"": {
      ""get"": {
        ""summary"": ""Retrieves a list of resources"",
        ""description"": ""Retrieves a list of resources"",
        ""operationId"": ""GetResources"",
        ""responses"": {
          ""500"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      }
    }
  },
  ""responses"": {
    ""GeneralError"": {
      ""description"": ""General error"",
      ""schema"": {
        ""type"": ""string""
      }
    }
  }
}";

            //// Act
            var document = await SwaggerDocument.FromJsonAsync(json);
            json = document.ToJson();

            //// Assert
            Assert.NotNull(document);
            Assert.NotNull(document.Operations.First().Operation.ActualResponses["500"].Schema);

            Assert.Contains(@"""$ref"": ""#/responses/GeneralError""", json);
        }

        [Fact]
        public async Task When_parameter_is_referenced_then_it_should_be_resolved()
        {
            //// Arrange
            var json = @"
{
  ""swagger"": ""2.0"",
  ""paths"": {
    ""/resources"": {
      ""get"": {
        ""summary"": ""Retrieves a list of resources"",
        ""description"": ""Retrieves a list of resources"",
        ""operationId"": ""GetResources"",
        ""parameters"": [
            {
                ""$ref"": ""#/parameters/Foo""
            }
        ],
        ""responses"": {
          ""500"": { }
        }
      }
    }
  },
  ""parameters"": {
    ""Foo"": {
      ""name"": ""foo"",
      ""type"": ""string""
    }
  }
}";

            //// Act
            var document = await SwaggerDocument.FromJsonAsync(json);
            json = document.ToJson();

            //// Assert
            Assert.Equal("foo", document.Operations.First().Operation.ActualParameters.First().Name);
            Assert.Contains(@"""$ref"": ""#/parameters/Foo""", json);
        }
    }
}