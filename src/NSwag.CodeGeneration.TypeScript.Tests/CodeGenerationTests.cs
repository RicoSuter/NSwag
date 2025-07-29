using System.ComponentModel.DataAnnotations;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests;

public class CodeGenerationTests
{
    [Fact]
    public async Task When_path_starts_with_numeric_can_generate_client()
    {
        // Arrange
        const string json = """
                            {
                              "openapi": "3.0.1",
                              "info": {
                                "title": "My.API | v1",
                                "version": "1.0.0"
                              },
                              "servers": [
                                {
                                  "url": "https://myapi.centralus-01.azurewebsites.net/"
                                }
                              ],
                              "paths": {
                                "/manage/2fa": {
                                  "post": {
                                    "tags": [
                                      "My.API"
                                    ],
                                    "requestBody": {
                                      "content": {
                                        "application/json": {
                                          "schema": {
                                            "$ref": "#/components/schemas/TwoFactorRequest"
                                          }
                                        }
                                      },
                                      "required": true
                                    },
                                    "responses": {
                                      "200": {
                                        "description": "OK",
                                        "content": {
                                          "application/json": {
                                            "schema": {
                                              "$ref": "#/components/schemas/TwoFactorResponse"
                                            }
                                          }
                                        }
                                      },
                                      "400": {
                                        "description": "Bad Request",
                                        "content": {
                                          "application/problem+json": {
                                            "schema": {
                                              "$ref": "#/components/schemas/HttpValidationProblemDetails"
                                            }
                                          }
                                        }
                                      },
                                      "404": {
                                        "description": "Not Found"
                                      }
                                    }
                                  }
                                }
                              },
                              "components": {
                                "schemas": {
                                  "HttpValidationProblemDetails": {
                                    "type": "object",
                                    "properties": {
                                      "type": {
                                        "type": "string",
                                        "nullable": true
                                      },
                                      "title": {
                                        "type": "string",
                                        "nullable": true
                                      },
                                      "status": {
                                        "type": "integer",
                                        "format": "int32",
                                        "nullable": true
                                      },
                                      "detail": {
                                        "type": "string",
                                        "nullable": true
                                      },
                                      "instance": {
                                        "type": "string",
                                        "nullable": true
                                      },
                                      "errors": {
                                        "type": "object",
                                        "additionalProperties": {
                                          "type": "array",
                                          "items": {
                                            "type": "string"
                                          }
                                        }
                                      }
                                    }
                                  },
                                  "TwoFactorRequest": {
                                    "type": "object",
                                    "properties": {
                                      "enable": {
                                        "type": "boolean",
                                        "nullable": true
                                      },
                                      "twoFactorCode": {
                                        "type": "string",
                                        "nullable": true
                                      },
                                      "resetSharedKey": {
                                        "type": "boolean"
                                      },
                                      "resetRecoveryCodes": {
                                        "type": "boolean"
                                      },
                                      "forgetMachine": {
                                        "type": "boolean"
                                      }
                                    }
                                  },
                                  "TwoFactorResponse": {
                                    "required": [
                                      "sharedKey",
                                      "recoveryCodesLeft",
                                      "isTwoFactorEnabled",
                                      "isMachineRemembered"
                                    ],
                                    "type": "object",
                                    "properties": {
                                      "sharedKey": {
                                        "type": "string"
                                      },
                                      "recoveryCodesLeft": {
                                        "type": "integer",
                                        "format": "int32"
                                      },
                                      "recoveryCodes": {
                                        "type": "array",
                                        "items": {
                                          "type": "string"
                                        },
                                        "nullable": true
                                      },
                                      "isTwoFactorEnabled": {
                                        "type": "boolean"
                                      },
                                      "isMachineRemembered": {
                                        "type": "boolean"
                                      }
                                    }
                                  }
                                }
                              },
                              "tags": [
                                {
                                  "name": "My.API"
                                }
                              ]
                            }
                            """;

        var document = await OpenApiDocument.FromJsonAsync(json);

        // Act
        var codeGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings());

        var code = codeGenerator.GenerateFile();

        // Assert
        await VerifyHelper.Verify(code);
        TypeScriptCompiler.AssertCompile(code);
    }


    [Fact]
    public async Task When_generating_TypeScript_code_then_output_contains_expected_classes()
    {
        // Arrange
        var document = CreateDocument();

        // Act
        var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
        {
            ClassName = "MyClass",
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface
            }
        });
        var code = generator.GenerateFile();

        // Assert
        await VerifyHelper.Verify(code);
        TypeScriptCompiler.AssertCompile(code);
    }

    private static OpenApiDocument CreateDocument()
    {
        var document = new OpenApiDocument();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings();
        var generator = new JsonSchemaGenerator(settings);

        document.Paths["/Person"] = new OpenApiPathItem
        {
            [OpenApiOperationMethod.Get] = new OpenApiOperation
            {
                Responses =
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Schema = new JsonSchema
                            {
                                Reference = generator.Generate(typeof(Person), new OpenApiSchemaResolver(document, settings))
                            }
                        }
                    }
                }
            }
        };
        return document;
    }

    public class Person
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public Sex Sex { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }
    }

    public enum Sex
    {
        Male,
        Female
    }
}