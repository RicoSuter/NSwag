using System.ComponentModel.DataAnnotations;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class CodeGenerationTests
    {
        [Fact]
        public async Task When_generating_CSharp_code_then_output_contains_expected_classes()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings { ClassName = "MyClass" };
            settings.CSharpGeneratorSettings.Namespace = "MyNamespace";

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_generating_CSharp_code_with_SystemTextJson_then_output_contains_expected_code()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_generating_CSharp_code_with_SystemTextJson_and_JsonSerializerSettingsTransformationMethod_then_output_contains_expected_code()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
            settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod = "TestJsonSerializerSettingsTransformationMethod";

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
namespace MyNamespace
{
    partial class Client
    {
        static global::System.Text.Json.JsonSerializerOptions TestJsonSerializerSettingsTransformationMethod(global::System.Text.Json.JsonSerializerOptions settings) { return settings; }
    }
}
");
        }

        [Fact]
        public async Task When_generating_CSharp_code_with_NewtonsoftJson_and_JsonSerializerSettingsTransformationMethod_then_output_contains_expected_code()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.NewtonsoftJson;
            settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod = "TestJsonSerializerSettingsTransformationMethod";

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
namespace MyNamespace
{
    partial class Client
    {
        static global::Newtonsoft.Json.JsonSerializerSettings TestJsonSerializerSettingsTransformationMethod(global::Newtonsoft.Json.JsonSerializerSettings settings) { return settings; }
    }
}
");
        }

        [Fact]
        public async Task When_generating_CSharp_code_with_SystemTextJson_and_JsonConverters_then_output_contains_expected_code()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
            settings.CSharpGeneratorSettings.JsonConverters = ["CustomConverter1", "CustomConverter2"];

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
#nullable disable
namespace MyNamespace {
    class CustomConverter1 : global::System.Text.Json.Serialization.JsonConverter<Client>
    {
        public override Client Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) { return default(Client); }
        public override void Write(global::System.Text.Json.Utf8JsonWriter writer, Client value, global::System.Text.Json.JsonSerializerOptions options) { }
    }
    class CustomConverter2 : CustomConverter1 { }
}
");
        }

        [Fact]
        public async Task When_generating_CSharp_code_with_SystemTextJson_and_GenerateJsonMethods_and_JsonConverters_then_ToJson_and_FromJson_contains_expected_code()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
            settings.CSharpGeneratorSettings.JsonConverters = ["CustomConverter1", "CustomConverter2"];
            settings.CSharpGeneratorSettings.GenerateJsonMethods = true;

            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
#nullable disable
namespace MyNamespace {
    class CustomConverter1 : global::System.Text.Json.Serialization.JsonConverter<Client>
    {
        public override Client Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options) { return default(Client); }
        public override void Write(global::System.Text.Json.Utf8JsonWriter writer, Client value, global::System.Text.Json.JsonSerializerOptions options) { }
    }
    class CustomConverter2 : CustomConverter1 { }
}
");
        }

        [Fact]
        public async Task When_Success_Response_contains_multiple_content_types_prioritizes_wildcard()
        {
            // Arrange
            var document = CreateDocument();
            var operation = document.Paths["/Person"][OpenApiOperationMethod.Get];

            operation.Responses["200"].Content.Clear();

            operation.Responses["200"].Content.Add("application/xml", new OpenApiMediaType
            {
                Schema = new JsonSchema { Type = JsonObjectType.Object }
            });

            operation.Responses["200"].Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new JsonSchema { Type = JsonObjectType.Object }
            });

            operation.Responses["200"].Content.Add("*/*", new OpenApiMediaType
            {
                Schema = new JsonSchema { Type = JsonObjectType.Object }
            });

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_Success_Response_contains_multiple_content_types_prioritizes_json()
        {
            // Arrange
            var document = CreateDocument();
            var operation = document.Paths["/Person"][OpenApiOperationMethod.Get];

            operation.Responses["200"].Content.Clear();

            operation.Responses["200"].Content.Add("application/xml", new OpenApiMediaType
            {
                Schema = new JsonSchema { Type = JsonObjectType.Object }
            });

            operation.Responses["200"].Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new JsonSchema { Type = JsonObjectType.Object }
            });

            // Act
            var settings = new CSharpClientGeneratorSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = NJsonSchema.CodeGeneration.CSharp.CSharpJsonLibrary.SystemTextJson;
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

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
            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());

            var code = codeGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
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
    }

    public class Person
    {
        [Required] public string FirstName { get; set; }

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