using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class QueryParameterTests
    {
        [Fact]
        public async Task When_query_parameter_is_set_to_explode_and_style_is_form_object_parameters_are_expanded()
        {
            var spec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Query params tests""
  },
  ""servers"": [
    {
      ""url"": ""http://localhost:8080""
    }
  ],
  ""paths"": {
    ""/settings"": {
      ""get"": {
        ""summary"": ""List all settings"",
        ""operationId"": ""listSettings"",
        ""parameters"": [
          {
            ""name"": ""paging"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""style"": ""form"",
            ""explode"": true,
            ""schema"": {
              ""$ref"": ""#/components/schemas/Paging""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""A paged array of settings""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""Paging"": {
        ""type"": ""object"",
        ""properties"": {
          ""page"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          },
          ""limit"": {
            ""type"": ""integer"",
            ""format"": ""int32""
          }
        }
      }
    }
  }
}
";

            var document = await OpenApiDocument.FromJsonAsync(spec);

            // Act
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_query_parameter_is_untyped_free_form_object_parameters_are_expanded()
        {
            var spec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Query params tests""
  },
  ""servers"": [
    {
      ""url"": ""http://localhost:8080""
    }
  ],
  ""paths"": {
    ""/settings"": {
      ""get"": {
        ""summary"": ""List all settings"",
        ""operationId"": ""listSettings"",
        ""parameters"": [
          {
            ""name"": ""extendedProperties"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""style"": ""form"",
            ""explode"": true,
            ""schema"": {
              ""$ref"": ""#/components/schemas/ExtendedProperties""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""An array of settings""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""ExtendedProperties"": {
        ""type"": ""object"",
        ""additionalProperties"": true
      }
    }
  }
}
";

            var document = await OpenApiDocument.FromJsonAsync(spec);

            // Act
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_query_parameter_is_typed_free_form_object_parameters_are_expanded()
        {
            var spec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Query params tests""
  },
  ""servers"": [
    {
      ""url"": ""http://localhost:8080""
    }
  ],
  ""paths"": {
    ""/settings"": {
      ""get"": {
        ""summary"": ""List all settings"",
        ""operationId"": ""listSettings"",
        ""parameters"": [
          {
            ""name"": ""extendedProperties"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""style"": ""form"",
            ""explode"": true,
            ""schema"": {
              ""$ref"": ""#/components/schemas/ExtendedProperties""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""An array of settings""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""ExtendedProperties"": {
        ""type"": ""object"",
        ""additionalProperties"": {
          ""type"": ""string""
        }
      }
    }
  }
}
";

            var document = await OpenApiDocument.FromJsonAsync(spec);

            // Act
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_query_parameter_is_mixed_free_form_object_parameters_are_expanded()
        {
            var spec = @"{
  ""openapi"": ""3.0.0"",
  ""info"": {
    ""version"": ""1.0.0"",
    ""title"": ""Query params tests""
  },
  ""servers"": [
    {
      ""url"": ""http://localhost:8080""
    }
  ],
  ""paths"": {
    ""/settings"": {
      ""get"": {
        ""summary"": ""List all settings"",
        ""operationId"": ""listSettings"",
        ""parameters"": [
          {
            ""name"": ""limit"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""schema"": {
              ""type"": ""integer"",
              ""format"": ""int32""
            }
          },
          {
            ""name"": ""extendedProperties"",
            ""in"": ""query"",
            ""description"": ""list setting filter"",
            ""required"": false,
            ""style"": ""form"",
            ""explode"": true,
            ""schema"": {
              ""$ref"": ""#/components/schemas/ExtendedProperties""
            }
          }
        ],
        ""responses"": {
          ""200"": {
            ""description"": ""An array of settings""
          }
        }
      }
    }
  },
  ""components"": {
    ""schemas"": {
      ""ExtendedProperties"": {
        ""type"": ""object"",
        ""additionalProperties"": {
          ""type"": ""string""
        },
        ""properties"": {
          ""default"": {
            ""type"": ""string""
          }
        }
      }
    }
  }
}
";

            var document = await OpenApiDocument.FromJsonAsync(spec);

            // Act
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}