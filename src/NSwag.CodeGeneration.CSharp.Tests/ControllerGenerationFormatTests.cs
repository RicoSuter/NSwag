using System.Text.RegularExpressions;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ControllerGenerationFormatTests
    {
        [Fact]
        public async Task When_controllergenerationformat_abstract_then_abstractcontroller_is_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllergenerationformat_partial_then_partialcontroller_is_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial,
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_abstract_then_actiontype_is_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_partial_then_actiontype_is_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllergenerationformat_notsetted_then_partialcontroller_is_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings());
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_partialcontroller_is_generated_with_frombody_attribute()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings();

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_abstractcontroller_is_generated_with_frombody_attribute()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_operationid_then_route_attribute_name_specified()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.OperationId
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_none_then_route_attribute_name_not_specified()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.None
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controller_has_operations_with_required_parameters_then_partialcontroller_is_generated_with_bindrequired_attribute()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                GenerateModelValidationAttributes = true,
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controller_has_operations_with_required_parameters_then_abstractcontroller_is_generated_with_bindrequired_attribute()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                GenerateModelValidationAttributes = true,
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_the_generation_of_dto_classes_are_disabled_then_file_is_generated_without_any_dto_clasess()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                GenerateDtoTypes = false
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
namespace MyNamespace
{
    public class ComplexType {}
    public class ComplexTypeResponse {}
}
");
        }

        private static OpenApiDocument GetOpenApiDocument()
        {
            JsonSchema complexTypeSchema = new JsonSchema();
            complexTypeSchema.Title = "ComplexType";
            complexTypeSchema.Properties["Prop1"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true };
            complexTypeSchema.Properties["Prop2"] = new JsonSchemaProperty { Type = JsonObjectType.Integer, IsRequired = true };
            complexTypeSchema.Properties["Prop3"] = new JsonSchemaProperty { Type = JsonObjectType.Boolean, IsRequired = true };
            complexTypeSchema.Properties["Prop4"] = new JsonSchemaProperty { Type = JsonObjectType.Object, Reference = complexTypeSchema, IsRequired = true };

            JsonSchema complexTypeReponseSchema = new JsonSchema();
            complexTypeReponseSchema.Title = "ComplexTypeResponse";
            complexTypeReponseSchema.Properties["Prop1"] = new JsonSchemaProperty { Type = JsonObjectType.String, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop2"] = new JsonSchemaProperty { Type = JsonObjectType.Integer, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop3"] = new JsonSchemaProperty { Type = JsonObjectType.Boolean, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop4"] = new JsonSchemaProperty { Type = JsonObjectType.Object, Reference = complexTypeSchema, IsRequired = true };

            var typeString = NewtonsoftJsonSchemaGenerator.FromType<string>();

            var document = new OpenApiDocument();
            document.Paths["Foo"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Get,
                    new OpenApiOperation {
                        OperationId = "Test_Foo",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "test",
                                IsRequired = false,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.String
                            },
                            new OpenApiParameter {
                                Name = "test2",
                                IsRequired = false,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            }
                        },
                        Responses =
                        {
                            new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse
                            {
                                Schema = typeString
                            })
                        }
                    }
                }
            };

            document.Paths["FooRequired"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Get,
                    new OpenApiOperation {
                        OperationId = "Test_FooRequired",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "test",
                                IsRequired = true,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.String
                            },
                            new OpenApiParameter {
                                Name = "test2",
                                IsRequired = true,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            }
                        },
                        Responses =
                        {
                            new KeyValuePair<string, OpenApiResponse>("200", new OpenApiResponse
                            {
                                Schema = typeString
                            })
                        }
                    }
                }
            };

            document.Paths["Bar"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation {
                        OperationId = "Test_Bar",
                    }
                }
            };

            document.Paths["HeaderParam"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation {
                        OperationId = "Test_HeaderParam",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "comesFromHeader",
                                Kind = OpenApiParameterKind.Header,
                                Type = JsonObjectType.String
                            }
                        }
                    }
                }
            };

            document.Paths["HeaderParamRequired"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation {
                        OperationId = "Test_HeaderParamRequired",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "comes-from-header",
                                IsRequired = true,
                                Kind = OpenApiParameterKind.Header,
                                Type = JsonObjectType.String
                            }
                        },
                        Tags = ["Secondary"]
                    }
                }
            };

            document.Paths["Complex"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation {
                        OperationId = "Test_Complex",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "complexType",
                                IsRequired = false,
                                Kind = OpenApiParameterKind.Body,
                                Type = JsonObjectType.Object,
                                Reference = complexTypeSchema
                            }
                        }
                    }
                }
            };

            document.Paths["ComplexRequired"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Post,
                    new OpenApiOperation {
                        OperationId = "Test_ComplexRequired",
                        Parameters = {
                            new OpenApiParameter {
                                Name = "complexType",
                                IsRequired = true,
                                Kind = OpenApiParameterKind.Body,
                                Type = JsonObjectType.Object,
                                Reference = complexTypeSchema
                            }
                        },
                        Responses = {
                            new System.Collections.Generic.KeyValuePair<string, OpenApiResponse>
                            (
                                "200", new OpenApiResponse
                                {
                                    Reference = new OpenApiResponse
                                    {
                                        Schema = complexTypeReponseSchema
                                    }
                                }
                            )
                        }
                    }
                }
            };

            document.Definitions["ComplexType"] = complexTypeSchema;
            document.Definitions["ComplexTypeResponse"] = complexTypeReponseSchema;
            return document;
        }

        [Fact]
        public async Task When_controllertarget_aspnet_and_multiple_controllers_then_only_single_custom_fromheader_generated()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                ControllerTarget = CSharpControllerTarget.AspNet,
                ControllerStyle = CSharpControllerStyle.Abstract,
                OperationNameGenerator = new MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator()
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            var fromHeaderCustomAttributeCount = Regex.Matches(code, "public class FromHeaderAttribute :").Count;
            Assert.Equal(1, fromHeaderCustomAttributeCount);

            var fromHeaderCustomBindingCount = Regex.Matches(code, "public class FromHeaderBinding :").Count;
            Assert.Equal(1, fromHeaderCustomBindingCount);

            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllertarget_aspnetcore_then_use_builtin_fromheader()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                ControllerTarget = CSharpControllerTarget.AspNetCore
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_header_parameter_then_partialcontroller_is_generated_with_fromheader_attribute()
        {
            // Arrange
            var document = GetOpenApiDocument();
            var settings = new CSharpControllerGeneratorSettings
            {
                ControllerTarget = CSharpControllerTarget.AspNet
            };

            // Act
            var codeGen = new CSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);

            CSharpCompiler.AssertCompile(code);
        }
    }
}
