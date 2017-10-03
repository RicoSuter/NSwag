using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace NSwag.Tests.Specification
{
    [TestClass]
    public class DocumentReferenceTests
    {
        [TestMethod]
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
            Assert.IsNotNull(document, "Document not parsed");
            Assert.IsNotNull(document.Operations.First().Operation.ActualResponses["500"].Schema, "Response schema not parsed");

            Assert.IsTrue(json.Contains(@"""$ref"": ""#/responses/GeneralError"""));
        }

        [TestMethod]
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
            Assert.AreEqual("foo", document.Operations.First().Operation.ActualParameters.First().Name);
            Assert.IsTrue(json.Contains(@"""$ref"": ""#/parameters/Foo"""));
        }
    }
}