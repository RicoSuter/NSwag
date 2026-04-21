using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.Tests;
using Parlot;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class IssueTests
    {
        [Fact]
        public async Task Issue4587_allow_nullable_path_parameters()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/api/1/{param}"": {
      ""get"": {
        ""parameters"": [
          {
            ""name"": ""param"",
            ""in"": ""path"",
            ""required"": false,
            ""schema"": {
              ""type"": ""string"",
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/api/2/{param}/info"": {
      ""get"": {
        ""parameters"": [
          {
            ""name"": ""param"",
            ""in"": ""path"",
            ""required"": false,
            ""schema"": {
              ""type"": ""string"",
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
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
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task Issue5209_Single_character_path_without_parameter_causes_compilation_error_when_character_is_quote()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.3"",
  ""info"": {
    ""title"": ""Reproduce CSharp codegen edge case bug"",
    ""version"": ""1.0""
  },
  ""paths"": {
    ""/v1/this/is/a/test/path/?q='{param1}'"": {
      ""get"": {
        ""summary"": ""Query a thing"",
        ""operationId"": ""GetThing"",
        ""parameters"": [
          {
            ""$ref"": ""#/components/parameters/param1Param""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""OK""
          }
        }
      }
    }
  },
  ""components"": {
    ""parameters"": {
      ""param1Param"": {
        ""in"": ""path"",
        ""name"": ""param1"",
        ""required"": true,
        ""schema"": {
          ""type"": ""string""
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
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task Issue5260_compilation_error_in_query_string_array_parameters()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/queryProductOrder1"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder1"",
        ""parameters"": [
          {
            ""name"": ""destinationFolderId"",
            ""in"": ""query"",
            ""required"": true,
            ""style"": ""form"",
            ""explode"": false,
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/queryProductOrder2"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder2"",
        ""parameters"": [
          {
            ""name"": ""orderStatus"",
            ""in"": ""query"",
            ""style"": ""form"",
            ""explode"": false,
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string"",
                ""enum"": [
                  ""Draft"",
                  ""Submitted"",
                  ""Running"",
                  ""Completed"",
                  ""Cancelled"",
                  ""In Error"",
                  ""Suspended"",
                  ""Order Configuration Queued""
                ]
              }
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/queryProductOrder3"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder3"",
        ""parameters"": [
          {
            ""name"": ""orderStatus"",
            ""in"": ""query"",
            ""style"": ""form"",
            ""explode"": false,
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string"",
                ""enum"": [
                  ""Draft"",
                  ""Submitted"",
                  ""Running"",
                  ""Completed"",
                  ""Cancelled"",
                  ""In Error"",
                  ""Suspended"",
                  ""Order Configuration Queued""
                ]
              }
            }
          },
          {
            ""name"": ""destinationFolderId"",
            ""in"": ""query"",
            ""required"": true,
            ""style"": ""form"",
            ""explode"": false,
            ""schema"": {
              ""type"": ""array"",
              ""items"": {
                ""type"": ""string""
              }
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
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
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task Issue5260_compilation_error_in_query_string_dictionary_parameters()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""info"": {
    ""title"": ""Dynamic Dictionary Query API"",
    ""version"": ""1.0.0"",
    ""description"": ""An endpoint that accepts arbitrary key-value pairs in the query string""
  },
  ""paths"": {
    ""/dynamicQuery"": {
      ""get"": {
        ""summary"": ""Accept arbitrary key-value pairs via query string"",
        ""operationId"": ""getDynamicQuery"",
        ""parameters"": [
          {
            ""name"": ""params"",
            ""in"": ""query"",
            ""style"": ""deepObject"",
            ""explode"": true,
            ""schema"": {
              ""type"": ""object"",
              ""additionalProperties"": {
                ""type"": ""string""
              }
            },
            ""description"": ""Dictionary of arbitrary key-value pairs""
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""Success"",
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""object"",
                  ""additionalProperties"": {
                    ""type"": ""string""
                  }
                }
              }
            }
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
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task Issue5260_compilation_error_in_query_string_deep_object_parameters()
        {
            // Arrange
            var swagger =
@"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/queryProductOrder1"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder1"",
        ""parameters"": [
          {
            ""name"": ""params"",
            ""in"": ""query"",
            ""required"": true,
            ""style"": ""deepObject"",
            ""explode"": true,
            ""schema"": {
              ""type"": ""object"",
              ""properties"": {
                ""destinationFolderId"": {
                  ""type"": ""string"",
                }
              },
              ""required"": [""destinationFolderId""]
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/queryProductOrder2"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder2"",
        ""parameters"": [
          {
            ""name"": ""params"",
            ""in"": ""query"",
            ""style"": ""deepObject"",
            ""explode"": true,
            ""schema"": {
              ""type"": ""object"",
              ""properties"": {
                ""orderStatus"": {
                  ""type"": ""string"",
                  ""enum"": [
                    ""Draft"",
                    ""Submitted"",
                    ""Running"",
                    ""Completed"",
                    ""Cancelled"",
                    ""In Error"",
                    ""Suspended"",
                    ""Order Configuration Queued""
                  ]
                }
              }
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
          }
        }
      }
    },
    ""/queryProductOrder3"": {
      ""get"": {
        ""tags"": [
          ""Order""
        ],
        ""operationId"": ""getOrder3"",
        ""parameters"": [
          {
            ""name"": ""params"",
            ""in"": ""query"",
            ""required"": true,
            ""style"": ""deepObject"",
            ""explode"": true,
            ""schema"": {
              ""type"": ""object"",
              ""properties"": {
                ""orderStatus"": {
                  ""type"": ""string"",
                  ""enum"": [
                    ""Draft"",
                    ""Submitted"",
                    ""Running"",
                    ""Completed"",
                    ""Cancelled"",
                    ""In Error"",
                    ""Suspended"",
                    ""Order Configuration Queued""
                  ]
                },
                ""destinationFolderId"": {
                  ""type"": ""string"",
                }
              },
              ""required"": [""destinationFolderId""]
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""content"": {
              ""application/json"": {
                ""schema"": {
                  ""type"": ""string""
                }
              }
            }
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
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}
