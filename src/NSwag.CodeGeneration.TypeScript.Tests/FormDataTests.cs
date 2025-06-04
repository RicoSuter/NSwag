using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class FormDataTests
    {
      [Fact]
        public async Task WhenInBody_FormUrlEncoded_DateOnly_is_converted_using_formatDate()
        {
          // Arrange
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
                              ""dateOnly"": {
                                ""type"": ""string"",
                                ""format"": ""date""
                              }
                            }
                          }
                        },
                        ""application/x-www-form-urlencoded"": {
                        }
                      }
                    },
                    ""responses"" : {
                      ""default"" : {
                        ""description"" : ""successful operation""
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator =
                new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains("content_ += encodeURIComponent(\"dateOnly\") + \"=\" + encodeURIComponent(dateOnly ? formatDate(dateOnly) : \"\")", code);
        }
        
        [Fact]
        public async Task WhenInBody_FormUrlEncoded_DateOnlyArray_is_converted_using_formatDate()
        {
          // Arrange
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
                              ""dateOnlyArray"": {
                                ""type"": ""array"",
                                ""items"": {
                                    ""type"": ""string"",
                                    ""format"": ""date""
                                }
                              }
                            }
                          }
                        },
                        ""application/x-www-form-urlencoded"": {
                        }
                      }
                    },
                    ""responses"" : {
                      ""default"" : {
                        ""description"" : ""successful operation""
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator =
                new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains("dateOnlyArray && dateOnlyArray.forEach(item_ => { content_ += encodeURIComponent(\"dateOnlyArray\") + \"=\" + encodeURIComponent(item_ ? formatDate(item_) : \"null\") + \"&\"; });", code);
        }

        [Fact]
        public async Task WhenInBody_MultipartFormData_DateOnly_is_converted_using_formatDate()
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
                              ""dateOnly"": {
                                ""type"": ""string"",
                                ""format"": ""date"",
                              },
                              ""test"": {
                                ""type"": ""string""
                              }
                            }
                          }
                        }
                      }
                    },
                    ""responses"" : {
                      ""default"" : {
                        ""description"" : ""successful operation""
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator =
                new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains("const content_ = new FormData();", code);
            Assert.Contains("content_.append(\"dateOnly\", formatDate(dateOnly));", code);
        }

        [Fact]
        public async Task WhenInBody_MultipartFormData_DateOnlyArray_is_converted_using_formatDate()
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
                              ""dateOnlyArray"": {
                                ""type"": ""array"",
                                ""items"": {
                                    ""type"": ""string"",
                                    ""format"": ""date""
                                }
                              },
                              ""test"": {
                                ""type"": ""string""
                              }
                            }
                          }
                        }
                      }
                    },
                    ""responses"" : {
                      ""default"" : {
                        ""description"" : ""successful operation""
                      }
                    }
                  }
                },
    
              },
              ""components"": {}
            }";

            var document = await OpenApiDocument.FromJsonAsync(json);

            // Act
            var codeGenerator =
                new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            // Assert
            Assert.Contains("const content_ = new FormData();", code);
            Assert.Contains(
                "dateOnlyArray.forEach(item_ => content_.append(\"dateOnlyArray\", formatDate(item_)));",
                code);
        }
    }
}