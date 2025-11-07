using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ArrayParameterTests
    {
        [Fact]
        public async Task When_parameter_is_array_then_CSharp_is_correct()
        {
            // Arrange
            var swagger = @"{
  ""swagger"" : ""2.0"",
  ""info"" : {
    ""version"" : ""1.0.2"",
    ""title"" : ""Test API""
  },
  ""host"" : ""localhost:8080"",
  ""basePath"" : ""/"",
  ""tags"" : [ {
    ""name"" : ""api""
  } ],
  ""schemes"" : [ ""http"" ],
  ""paths"" : {
     ""/removeElement"" : {
      ""delete"" : {
        ""tags"" : [ ""api"" ],
        ""summary"" : ""Removes elements"",
        ""description"" : ""Removes elements"",
        ""operationId"" : ""removeElement"",
        ""consumes"" : [ ""application/json"" ],
        ""produces"" : [ ""application/json"" ],
        ""parameters"" : [ {
          ""name"" : ""X-User"",
          ""in"" : ""header"",
          ""description"" : ""User identifier"",
          ""required"" : true,
          ""type"" : ""string""
        }, {
          ""name"" : ""elementId"",
          ""in"" : ""query"",
          ""description"" : ""The ids of existing elements that should be removed"",
          ""required"" : false,
          ""type"" : ""array"",
          ""items"" : {
            ""type"" : ""integer"",
            ""format"" : ""int64""
          },
        } ],
        ""responses"" : {
          ""default"" : {
            ""description"" : ""successful operation""
          }
        }
      }
    }
  },
    ""definitions"" : { }
}
";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Theory]
        [InlineData(@"""explode"": false,")]
        [InlineData(@"""style"": ""form"", ""explode"": false,")]
        public async Task When_explode_is_false_then_do_not_explode(string explode)
        {
            // Arrange
            var json = @"{
              ""x-generator"": ""NSwag v13.5.0.0 (NJsonSchema v10.1.15.0 (Newtonsoft.Json v11.0.0.0))"",
              ""openapi"": ""3.0.4"",
              ""info"": {
                ""title"": ""My Title"",
                ""version"": ""1.0.0""
              },
              ""paths"": {
                ""/removeElement"": {
                  ""delete"": {
                    ""tags"": [
                      ""api""
                    ],
                    ""operationId"": ""removeElement"",
                    ""parameters"": [
                      {
                        ""name"": ""X-User"",
                        ""in"": ""header"",
                        ""description"": ""User identifier"",
                        ""required"": true,
                        ""schema"": {
                          ""type"": ""string""
                        }
                      },
                      {
                        ""name"": ""elementId"",
                        ""in"": ""query"",
                        ""description"": ""The ids of existing elements that should be removed"",
                        ""required"": false,
                        " + explode + @"
                        ""schema"": {
                          ""type"": ""array"",
                          ""items"": {
                            ""type"": ""integer"",
                            ""format"": ""int64""
                          }
                        }
                      }
                    ],
                    ""responses"": {
                      ""200"": {
                        ""description"": """",
                        ""content"": {
                          ""application/octet-stream"": {
                            ""schema"": {
                              ""type"": ""string"",
                              ""format"": ""binary""
                            }
                          }
                        }
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code).UseParameters(explode);
            CSharpCompiler.AssertCompile(code);
        }

        [Theory]
        [InlineData("")]
        [InlineData(@"""style"": ""form"",")]
        [InlineData(@"""explode"": true,")]
        [InlineData(@"""style"": ""form"", ""explode"": true,")]
        public async Task When_explode_is_explicitly_or_implicitly_true_then_explode(string explode)
        {
            // Arrange
            var json = @"{
              ""x-generator"": ""NSwag v13.5.0.0 (NJsonSchema v10.1.15.0 (Newtonsoft.Json v11.0.0.0))"",
              ""openapi"": ""3.0.4"",
              ""info"": {
                ""title"": ""My Title"",
                ""version"": ""1.0.0""
              },
              ""paths"": {
                ""/removeElement"": {
                  ""delete"": {
                    ""tags"": [
                      ""api""
                    ],
                    ""operationId"": ""removeElement"",
                    ""parameters"": [
                      {
                        ""name"": ""X-User"",
                        ""in"": ""header"",
                        ""description"": ""User identifier"",
                        ""required"": true,
                        ""schema"": {
                          ""type"": ""string""
                        }
                      },
                      {
                        ""name"": ""elementId"",
                        ""in"": ""query"",
                        ""description"": ""The ids of existing elements that should be removed"",
                        ""required"": false,
                        " + explode + @"
                        ""schema"": {
                          ""type"": ""array"",
                          ""items"": {
                            ""type"": ""integer"",
                            ""format"": ""int64""
                          }
                        }
                      }
                    ],
                    ""responses"": {
                      ""200"": {
                        ""description"": """",
                        ""content"": {
                          ""application/octet-stream"": {
                            ""schema"": {
                              ""type"": ""string"",
                              ""format"": ""binary""
                            }
                          }
                        }
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";
            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code).UseParameters(explode);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_CollectionFormat_is_csv_then_do_not_explode()
        {
            // Arrange
            var swagger = @"{
  ""swagger"" : ""2.0"",
  ""info"" : {
    ""version"" : ""1.0.2"",
    ""title"" : ""Test API""
  },
  ""host"" : ""localhost:8080"",
  ""basePath"" : ""/"",
  ""tags"" : [ {
    ""name"" : ""api""
  } ],
  ""schemes"" : [ ""http"" ],
  ""paths"" : {
     ""/removeElement"" : {
      ""delete"" : {
        ""tags"" : [ ""api"" ],
        ""summary"" : ""Removes elements"",
        ""description"" : ""Removes elements"",
        ""operationId"" : ""removeElement"",
        ""consumes"" : [ ""application/json"" ],
        ""produces"" : [ ""application/json"" ],
        ""parameters"" : [ {
          ""name"" : ""X-User"",
          ""in"" : ""header"",
          ""description"" : ""User identifier"",
          ""required"" : true,
          ""type"" : ""string""
        }, {
          ""name"" : ""elementId"",
          ""in"" : ""query"",
          ""description"" : ""The ids of existing elements that should be removed"",
          ""required"" : false,
          ""type"" : ""array"",
          ""collectionFormat"": ""csv"",
          ""items"" : {
            ""type"" : ""integer"",
            ""format"" : ""int64""
          },
        } ],
        ""responses"" : {
          ""default"" : {
            ""description"" : ""successful operation""
          }
        }
      }
    }
  },
    ""definitions"" : { }
}
";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_CollectionFormat_is_multi_then_explode()
        {
            // Arrange
            var swagger = @"{
  ""swagger"" : ""2.0"",
  ""info"" : {
    ""version"" : ""1.0.2"",
    ""title"" : ""Test API""
  },
  ""host"" : ""localhost:8080"",
  ""basePath"" : ""/"",
  ""tags"" : [ {
    ""name"" : ""api""
  } ],
  ""schemes"" : [ ""http"" ],
  ""paths"" : {
     ""/removeElement"" : {
      ""delete"" : {
        ""tags"" : [ ""api"" ],
        ""summary"" : ""Removes elements"",
        ""description"" : ""Removes elements"",
        ""operationId"" : ""removeElement"",
        ""consumes"" : [ ""application/json"" ],
        ""produces"" : [ ""application/json"" ],
        ""parameters"" : [ {
          ""name"" : ""X-User"",
          ""in"" : ""header"",
          ""description"" : ""User identifier"",
          ""required"" : true,
          ""type"" : ""string""
        }, {
          ""name"" : ""elementId"",
          ""in"" : ""query"",
          ""description"" : ""The ids of existing elements that should be removed"",
          ""required"" : false,
          ""type"" : ""array"",
          ""collectionFormat"": ""multi"",
          ""items"" : {
            ""type"" : ""integer"",
            ""format"" : ""int64""
          },
        } ],
        ""responses"" : {
          ""default"" : {
            ""description"" : ""successful operation""
          }
        }
      }
    }
  },
    ""definitions"" : { }
}
";
            var document = await OpenApiDocument.FromJsonAsync(swagger);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task when_content_is_formdata_with_property_array_then_content_should_be_added_in_foreach_in_csharp()
        {
            var json = @"{
              ""x-generator"": ""NSwag v13.5.0.0 (NJsonSchema v10.1.15.0 (Newtonsoft.Json v11.0.0.0))"",
              ""openapi"": ""3.0.0"",
              ""info"": {
                ""title"": ""My Title"",
                ""version"": ""1.0.0""
              },
              ""paths"": {
                ""/api/FileUpload/UploadFile"": {
                  ""post"": {
                    ""tags"": [
                      ""FileUpload""
                    ],
                    ""operationId"": ""FileUpload_UploadFile"",
                    ""requestBody"": {
                      ""content"": {
                        ""multipart/form-data"": {
                          ""schema"": {
                            ""properties"": {
                              ""file"": {
                                ""type"": ""string"",
                                ""format"": ""binary""
                              },
                              ""arrayOfIds"": {
                                ""uniqueItems"": true,
                                ""type"": ""array"",
                                ""items"": {
                                  ""type"": ""string""
                                },
                                ""description"": ""Hash Set of of strings in the DTO request"",
                                ""nullable"": true
                              },
                              ""test"": {
                                ""type"": ""string""
                              }
                            }
                          }
                        }
                      }
                    },
                    ""responses"": {
                      ""200"": {
                        ""description"": """",
                        ""content"": {
                          ""application/octet-stream"": {
                            ""schema"": {
                              ""type"": ""string"",
                              ""format"": ""binary""
                            }
                          }
                        }
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}
