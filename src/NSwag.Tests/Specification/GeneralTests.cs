using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void WhenConvertingAndBackThenItShouldBeTheSame()
        {
            //// Arrange
            var json = _sampleServiceCode;

            //// Act
            var service = SwaggerService.FromJson(json);
            var json2 = service.ToJson();
            var reference = service.Paths["/pets"][SwaggerOperationMethod.get].Responses["200"].Schema.Item.SchemaReference;

            //// Assert
            Assert.IsNotNull(json2);
            Assert.IsNotNull(reference);
            Assert.AreEqual(3, reference.Properties.Count);
        }

        [TestMethod]
        public void WhenGeneratingOperationIdsThenMissingIdsAreGenerated()
        {
            //// Arrange
            var json = _sampleServiceCode;

            //// Act
            var service = SwaggerService.FromJson(json);
            service.GenerateOperationIds();

            //// Assert
            Assert.AreEqual("pets", service.Operations.First().Operation.OperationId);
        }

        private string _sampleServiceCode = 
@"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Swagger Petstore"",
    ""description"": ""A sample API that uses a petstore as an example to demonstrate features in the swagger-2.0 specification"",
    ""termsOfService"": ""http://swagger.io/terms/""
  },
  ""host"": ""petstore.swagger.io"",
  ""basePath"": ""/api"",
  ""schemes"": [
    ""http""
  ],
  ""consumes"": [
    ""application/json""
  ],
  ""produces"": [
    ""application/json""
  ],
  ""paths"": {
    ""/pets"": {
      ""get"": {
        ""description"": ""Returns all pets from the system that the user has access to"",
        ""produces"": [
          ""application/json""
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""A list of pets."",
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/definitions/Pet""
              }
            }
          }
        }
      }
    }
  },
  ""definitions"": {
    ""Pet"": {
      ""type"": ""object"",
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
        ""tag"": {
          ""type"": ""string""
        }
      }
    }
  }
}";
    }
}
