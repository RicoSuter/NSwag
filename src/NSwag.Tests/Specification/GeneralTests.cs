using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Tests
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        public void When_Swagger_is_loaded_from_url_then_it_works()
        {
            //// Arrange


            //// Act
            var swagger = SwaggerService.FromUrl("http://petstore.swagger.io/v2/swagger.json");

            //// Assert
            Assert.IsNotNull(swagger);
        }

        [TestMethod]
        public void WhenConvertingAndBackThenItShouldBeTheSame()
        {
            //// Arrange
            var json = _sampleServiceCode;

            //// Act
            var service = SwaggerService.FromJson(json);
            var json2 = service.ToJson();
            var reference = service.Paths["/pets"][SwaggerOperationMethod.Get].Responses["200"].Schema.Item.SchemaReference;

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

        [TestMethod]
        public void ExtensionDataTest()
        {
            //// Arrange
            var json = _jsonVendorExtensionData;

            //// Act
            var service = SwaggerService.FromJson(json);

            //// Assert
            Assert.IsNotNull(service.Operations.First().Operation.Responses["202"].ExtensionData);
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

        private string _jsonVendorExtensionData =
                    @"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": ""Swagger Test Sample"",
    ""description"": ""Swagger Test"",
    ""version"": ""1.0.0""
  },
  ""schemes"": [
    ""https""
  ],
  ""basePath"": ""/api/v1"",
  ""produces"": [
    ""application/json""
  ],
  ""consumes"": [
    ""application/json""
  ],
  ""host"": ""test.com"",
  ""paths"": {
    ""/12345/instances"": {
      ""post"": {
        ""summary"": ""Starts operation"",
        ""description"": ""Starts operation to trigger a task"",
        ""operationId"": ""123"",
        ""parameters"": [
          {
            ""name"": ""API Parameters"",
            ""required"": true,
            ""in"": ""body"",
            ""schema"": {
              ""type"": ""object"",
              ""properties"": {
                ""data"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""prop1"": {
                      ""title"": ""title 1"",
                      ""description"": ""description 1"",
                      ""type"": ""string""
                    },
                    ""prop2"": {
                      ""title"": ""title 2"",
                      ""description"": ""descripiton 2"",
                      ""type"": ""string""
                    }
                  }
                },
                ""options"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""callbackUrl"": {
                      ""title"": ""callbackUrl"",
                      ""description"": ""A Url to return the results back"",
                      ""type"": ""string""
                    }
                  }
                }
              }
            }
          },
          {
            ""name"": ""token"",
            ""type"": ""string"",
            ""in"": ""query"",
            ""description"": ""A security token""
          }
        ],
        ""responses"": {
          ""202"": {
            ""description"": ""Accepted"",
            ""x-callback-schema"": {
              ""type"": ""object"",
              ""properties"": {
                ""returnData"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""prop1"": {
                      ""title"": ""title 1"",
                      ""description"": ""description 1"",
                      ""type"": ""string""
                    },
                    ""prop2"": {
                      ""title"": ""title 2"",
                      ""description"": ""descripiton 2"",
                      ""type"": ""string""
                    }
                  }
                },
                ""workflow"": {
                  ""type"": ""object"",
                  ""properties"": {
                    ""id"": ""123"",
                    ""name"": ""Swagger Test""
                  }
                }
              }
            }
          },
          ""400"": {
            ""description"": ""Bad Request""
          },
          ""404"": {
            ""description"": ""Not Found""
          },
          ""429"": {
            ""description"": ""Too Many Requests""
          },
          ""503"": {
            ""description"": ""Service Unavailable - Overloaded""
          },
          ""default"": {
            ""description"": ""Unexpected Error""
          }
        }
      }
    }
  }
}";



    }
}
