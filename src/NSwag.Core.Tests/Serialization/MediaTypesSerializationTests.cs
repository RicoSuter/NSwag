using NJsonSchema;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class MediaTypesSerializationTests
    {
        [Fact]
        public void When_when_response_schema_and_example_is_set_then_it_is_serialized_correctly_in_Swagger()
        {
            //// Arrange
            var document = CreateDocument(JsonObjectType.String);

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Contains(
@"  ""paths"": {
    ""/foo"": {
      ""get"": {
        ""operationId"": ""foo"",
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""schema"": {
              ""type"": ""string""
            },
            ""examples"": 123
          }
        }
      }
    }
  }
}", json);
        }

        [Fact]
        public void When_when_response_schema_and_example_is_set_then_it_is_serialized_correctly_in_OpenApi()
        {
            //// Arrange
            var document = CreateDocument(JsonObjectType.String);

            //// Act
            var json = document.ToJson(SchemaType.OpenApi3);

            //// Assert
            Assert.Equal(
@"{
  ""openapi"": ""3.0"",
  ""info"": {
    ""title"": """",
    ""version"": """"
  },
  ""paths"": {
    ""/foo"": {
      ""get"": {
        ""operationId"": ""foo"",
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""content"": {
              ""application/json"": {
                ""example"": 123,
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {}
}", json);
        }

        [Fact]
        public void When_when_response_schema_and_example_is_set_as_file_then_it_is_serialized_correctly_in_Swagger()
        {
            //// Arrange
            var document = CreateDocument(JsonObjectType.File);

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Contains(
                @"  ""paths"": {
    ""/foo"": {
      ""get"": {
        ""operationId"": ""foo"",
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""schema"": {
              ""type"": ""file""
            },
            ""examples"": 123
          }
        }
      }
    }
  }
}", json);
        }

        [Fact]
        public void When_when_response_schema_and_example_is_set_as_file_then_it_is_serialized_correctly_in_OpenApi()
        {
            //// Arrange
            var document = CreateDocument(JsonObjectType.File);

            //// Act
            var json = document.ToJson(SchemaType.OpenApi3);

            //// Assert
            Assert.Equal(
                @"{
  ""openapi"": ""3.0"",
  ""info"": {
    ""title"": """",
    ""version"": """"
  },
  ""paths"": {
    ""/foo"": {
      ""get"": {
        ""operationId"": ""foo"",
        ""responses"": {
          ""200"": {
            ""description"": """",
            ""content"": {
              ""application/octet-stream"": {
                ""example"": 123,
                ""schema"": {
                  ""type"": ""file""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {}
}", json);
        }


        private static SwaggerDocument CreateDocument(JsonObjectType type)
        {
            var document = new SwaggerDocument
            {
                Paths =
                {
                    {
                        "/foo",
                        new SwaggerPathItem
                        {
                            {
                                SwaggerOperationMethod.Get, 
                                new SwaggerOperation
                                {
                                    Responses =
                                    {
                                        {
                                            "200", 
                                            new SwaggerResponse
                                            {
                                                Examples = 123,
                                                Schema = new JsonSchema4
                                                {
                                                    Type = type
                                                },
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            return document;
        }
    }
}