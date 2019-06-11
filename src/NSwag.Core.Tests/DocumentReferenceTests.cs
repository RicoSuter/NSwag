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
            var document = await OpenApiDocument.FromJsonAsync(json);
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
            var document = await OpenApiDocument.FromJsonAsync(json);
            json = document.ToJson();

            //// Assert
            Assert.Equal("foo", document.Operations.First().Operation.ActualParameters.First().Name);
            Assert.Contains(@"""$ref"": ""#/parameters/Foo""", json);
        }

        [Fact]
        public async Task When_parameter_references_schema_then_it_is_resolved()
        {
            //// Arrange
            var json = @"{
  ""openapi"": ""3.0.0"",
  ""servers"": [
    {
      ""url"": ""/api/v2"",
      ""description"": ""server""
    }
  ],
  ""paths"": {
    ""/subscriptions/{subscriptionId}"": {
      ""get"": {
        ""tags"": [
          ""subscriptions""
        ],
        ""summary"": ""get subscription details"",
        ""operationId"": ""subscriptions_details"",
        ""responses"": {
          
        }
      },
      ""parameters"": [
        {
          ""name"": ""subscriptionId"",
          ""in"": ""path"",
          ""required"": true,
          ""schema"": {
            ""$ref"": ""#/components/schemas/EntityId""
          }
        }
      ]
    }
  },
  ""components"": {
    ""schemas"": {
      ""EntityId"": {
        ""format"": ""secret"",
        ""type"": ""string"",
        ""example"": ""851D64D0-86DF-4BEC-8BA0-C81E326D735A""
      }
    }
  }
}";

            //// Act
            var document = await OpenApiDocument.FromJsonAsync(json);
            json = document.ToJson();

            //// Assert
            Assert.Equal("secret", document.Operations.First().Operation.ActualParameters.First().ActualSchema.Format);
        }
    }
}