using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ResponseTests
    {
        [Fact]
        public async Task When_response_references_object_in_inheritance_hierarchy_then_return_value_is_correct()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.0"",
  ""paths"": {
    ""/v1/exceptions/get"": {
      ""post"": {
        ""operationId"": ""Exceptions_GetException"",
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/ValidationException""
                }
              }
            }
          }
        }
      }
    }
  }, 
  ""components"": {
    ""schemas"": {
      ""BusinessException"": {
        ""type"": ""object"",
        ""additionalProperties"": false,
        ""properties"": {
          ""customerId"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""customerAlias"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""userId"": {
            ""type"": ""string"",
            ""nullable"": true
          }
        }
      },
      ""ValidationException"": {
        ""allOf"": [
          {
            ""$ref"": ""#/components/schemas/BusinessException""
          },
          {
            ""type"": ""object"",
            ""additionalProperties"": false
          }
        ]
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("System.Threading.Tasks.Task<ValidationException>", code);
            Assert.DoesNotContain("System.Threading.Tasks.Task<object>", code);
            Assert.Contains("class BusinessException", code);
            Assert.Contains("class ValidationException", code);
        }

        [Fact]
        public async Task When_response_is_referenced_any_then_class_is_generated()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.0"",
  ""paths"": {
    ""/v1/exceptions/get"": {
      ""post"": {
        ""operationId"": ""Exceptions_GetException"",
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/BusinessException""
                }
              }
            }
          }
        }
      }
    }
  }, 
  ""components"": {
    ""schemas"": {
      ""BusinessException"": {
        ""type"": ""object"",
        ""additionalProperties"": false
      }
    }
  }
}";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("System.Threading.Tasks.Task<BusinessException>", code);
            Assert.DoesNotContain("System.Threading.Tasks.Task<object>", code);
            Assert.Contains("class BusinessException", code);
        }

        [Fact]
        public async Task When_same_response_is_referenced_multiple_times_in_operation_then_class_is_generated()
        {
            // Arrange
            string json = @"{
  ""openapi"": ""3.0.0"",
  ""paths"": {
    ""/v1/exceptions/get"": {
      ""get"": {
        ""operationId"": ""Exceptions_GetException"",
        ""responses"": {
          ""200"": {
            ""$ref"": ""#/components/responses/BusinessExceptionResponse"",
            },
          ""400"": {
            ""$ref"": ""#/components/responses/BusinessExceptionResponse"",
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""BusinessException"": {
        ""type"": ""object"",
        ""additionalProperties"": false
      }
    },
    ""responses"": {
      ""BusinessExceptionResponse"": {
        ""description"": ""List of NSwagStudio bugs"",
        ""content"": {
          ""application/json"": {
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""$ref"": ""#/components/schemas/BusinessException""
              }
            }
          }
        }
      }
    }
  }
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("System.Threading.Tasks.Task<System.Collections.Generic.ICollection<BusinessException>>", code);
            Assert.Contains("class BusinessException", code);
        }

        [Fact]
        public async Task When_responses_produce_multiple_types()
        {
            // Arrange
            string json = @"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""WebApplication"",
    ""version"": ""1.0""
  },
  ""servers"": [
    {
      ""url"": ""/WebApplication""
    }
  ],
  ""paths"": {
    ""/Account/Authenticate"": {
      ""post"": {
        ""tags"": [
          ""Account""
        ],
        ""requestBody"": {
          ""content"": {
            ""application/json"": {
              ""schema"": {
                ""$ref"": ""#/components/schemas/AuthenticationRequest""
              }
            }
          }
        },
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""content"": {
              ""text/plain"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          },
          ""400"": {
            ""description"": ""Bad Request"",
            ""content"": {
              ""application/problem+json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/ValidationProblemDetails""
                }
              }
            }
          },
          ""401"": {
            ""description"": ""Unauthorized"",
            ""content"": {
              ""application/problem+json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/ProblemDetails""
                }
              }
            }
          },
          ""415"": {
            ""description"": ""Unsupported Media Type"",
            ""content"": {
              ""application/problem+json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/ProblemDetails""
                }
              }
            }
          },
          ""500"": {
            ""description"": ""Internal Server Error"",
            ""content"": {
              ""application/problem+json"": {
                ""schema"": {
                  ""$ref"": ""#/components/schemas/ProblemDetails""
                }
              }
            }
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""AuthenticationRequest"": {
        ""type"": ""object"",
        ""additionalProperties"": false
      },
      ""ValidationProblemDetails"": {
        ""type"": ""object"",
        ""properties"": {
          ""type"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""title"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""status"": {
            ""type"": ""integer"",
            ""format"": ""int32"",
            ""nullable"": true
          },
          ""detail"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""instance"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""errors"": {
            ""type"": ""object"",
            ""additionalProperties"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            },
            ""nullable"": true,
            ""readOnly"": true
          }
        },
        ""additionalProperties"": { }
      },
      ""ProblemDetails"": {
        ""type"": ""object"",
        ""properties"": {
          ""type"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""title"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""status"": {
            ""type"": ""integer"",
            ""format"": ""int32"",
            ""nullable"": true
          },
          ""detail"": {
            ""type"": ""string"",
            ""nullable"": true
          },
          ""instance"": {
            ""type"": ""string"",
            ""nullable"": true
          }
        },
        ""additionalProperties"": { }
      }
    }
  }
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("var result_ = (string)System.Convert.ChangeType(responseData_, typeof(string));", code);
            Assert.Contains("await ReadObjectResponseAsync<ValidationProblemDetails>", code);
            Assert.Contains("await ReadObjectResponseAsync<ProblemDetails>", code);
        }

        [Fact]
        public async Task When_responses_produce_primitive_types()
        {
            // Arrange
            string json = @"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": ""My Title"",
    ""version"": ""1.0.0""
  },
  ""host"": ""localhost:13452"",
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
    ""/api/Geo/UploadFile"": {
      ""post"": {
        ""operationId"": ""Geo_UploadFile"",
        ""consumes"": [
          ""multipart/form-data""
        ],
        ""parameters"": [
          {
            ""type"": ""file"",
            ""name"": ""file"",
            ""in"": ""formData"",
            ""x-nullable"": true
          }
        ],
        ""responses"": {
          ""200"": {
            ""x-nullable"": false,
            ""description"": """",
            ""schema"": {
              ""type"": ""boolean""
            }
          }
        }
      }
    },
    ""/api/Geo/PostDouble"": {
      ""post"": {
        ""operationId"": ""Geo_PostDouble"",
        ""parameters"": [
          {
            ""type"": ""number"",
            ""name"": ""value"",
            ""in"": ""query"",
            ""format"": ""double"",
            ""x-nullable"": true
          }
        ],
        ""responses"": {
          ""200"": {
            ""x-nullable"": true,
            ""description"": """",
            ""schema"": {
              ""type"": ""number"",
              ""format"": ""double""
            }
          }
        }
      }
    },
    ""/api/Persons/{id}/Name"": {
      ""get"": {
        ""operationId"": ""Persons_GetName"",
        ""parameters"": [
          {
            ""type"": ""string"",
            ""name"": ""id"",
            ""in"": ""path"",
            ""required"": true,
            ""description"": ""The person ID."",
            ""format"": ""guid"",
            ""x-nullable"": false
          }
        ],
        ""responses"": {
          ""200"": {
            ""x-nullable"": false,
            ""schema"": {
              ""type"": ""string""
            }
          }
        }
      }
    },
    ""/api/Persons/AddXml"": {
      ""post"": {
        ""operationId"": ""Persons_AddXml"",
        ""consumes"": [
          ""application/xml""
        ],
        ""parameters"": [
          {
            ""name"": ""person"",
            ""in"": ""body"",
            ""schema"": {
              ""type"": ""string"",
              ""x-nullable"": true
            },
            ""x-nullable"": true
          }
        ],
        ""responses"": {
          ""200"": {
            ""x-nullable"": true,
            ""description"": """",
            ""schema"": {
              ""type"": ""string""
            }
          }
        }
      }
    },
    ""/api/Persons/upload"": {
      ""post"": {
        ""operationId"": ""Persons_Upload"",
        ""consumes"": [
          ""application/octet-stream"",
          ""multipart/form-data""
        ],
        ""parameters"": [
          {
            ""name"": ""data"",
            ""in"": ""body"",
            ""schema"": {
              ""type"": ""string"",
              ""format"": ""binary"",
              ""x-nullable"": true
            },
            ""x-nullable"": true
          }
        ],
        ""responses"": {
          ""200"": {
            ""x-nullable"": true,
            ""description"": """",
            ""schema"": {
              ""type"": ""string"",
              ""format"": ""byte""
            }
          }
        }
      }
    }
  }
}";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.DoesNotContain("System.Convert.ChangeType(response", code);
            Assert.Contains("await ReadObjectResponseAsync<bool>", code);
            Assert.Contains("await ReadObjectResponseAsync<double?>", code);
            Assert.Contains("await ReadObjectResponseAsync<string>", code);
            Assert.Contains("await ReadObjectResponseAsync<byte[]>", code);
        }
    }
}
