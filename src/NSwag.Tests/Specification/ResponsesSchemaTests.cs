using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace NSwag.Tests.Specification
{
    [TestClass]
    public class ResponsesSchemaTests
    {
        [TestMethod]
        public async Task Refference_In_Response_ShouldBe_Resolved()
        {
            //// Arrange
            var json = @"
{
  ""swagger"": ""2.0"",
  ""info"": {
    ""version"": ""0.1.0"",
    ""title"": ""Test example"",
    ""description"": ""Test example""
  },
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""basePath"": ""/"",
  ""schemes"": [
    ""http""
  ],
  ""paths"": {
    ""/resources"": {
      ""get"": {
        ""summary"": ""Retrieves a list of resources"",
        ""description"": ""Retrieves a list of resources"",
        ""operationId"": ""GetResources"",
        ""responses"": {
          ""200"": {
            ""description"": ""Successful response"",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/Resource""
              }
            }
          },
          ""500"": {
            ""$ref"": ""#/responses/GeneralError""
          }
        }
      }
    }
  },
  ""definitions"": {
    ""Resource"": {
      ""description"": ""Some resource."",
      ""type"": ""object"",
      ""properties"": {
        ""name"": {
          ""type"": ""string""
        }
      }
    },
    ""Error"": {
      ""type"": ""object"",
      ""required"": [
        ""message""
      ],
      ""properties"": {
        ""message"": {
          ""type"": ""string"",
          ""description"": ""Error message""
        }
      }
    }
},
  ""responses"": {
    ""GeneralError"": {
      ""description"": ""General error"",
      ""schema"": {
        ""$ref"": ""#/definitions/Error""
      }
    }
  }
}";

            //// Act
            var document = await SwaggerDocument.FromJsonAsync(json);

            //// Assert
            Assert.IsNotNull(document, "Document not parsed");
            Assert.IsNotNull(document.Operations.First().Operation.Responses["500"].Schema, "Response schema not parsed");
        }
    }
}
