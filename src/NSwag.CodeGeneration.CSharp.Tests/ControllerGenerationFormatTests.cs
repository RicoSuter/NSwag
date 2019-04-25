using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ControllerGenerationFormatTests
    {
        [Fact]
        public async Task When_controllergenerationformat_abstract_then_abstractcontroller_is_generated()
        {
            //// Arrange
            var document = await GetSwaggerDocument();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.DoesNotContain("ITestController", code);
            Assert.DoesNotContain("private ITestController _implementation;", code);
            Assert.DoesNotContain("partial class TestController", code);
        }

        [Fact]
        public async Task When_controllergenerationformat_abstract_then_partialcontroller_is_generated()
        {
            //// Arrange
            var document = await GetSwaggerDocument();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial,
            });
            var code = codeGen.GenerateFile();

            //// Assert

            Assert.Contains("partial class TestController", code);
            Assert.Contains("ITestController", code);
            Assert.Contains("private ITestController _implementation;", code);
            Assert.DoesNotContain("abstract class TestController", code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_abstract_then_actiontype_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await GetSwaggerDocument();


            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("public abstract System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> Foo(string test, bool? test2);", code);
            Assert.Contains("public abstract System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> Bar();", code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_partial_then_actiontype_is_generated()
        {
            //// Arrange
            var document = await GetSwaggerDocument();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> FooAsync(string test, bool? test2);", code);
            Assert.Contains("public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> Foo(string test, bool? test2)", code);
            Assert.Contains("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> BarAsync();", code);
            Assert.Contains("public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> Bar()", code);
        }

        [Fact]
        public async Task When_controllergenerationformat_notsetted_then_partialcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await GetSwaggerDocument();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains("ITestController", code);
            Assert.Contains("private ITestController _implementation;", code);
            Assert.DoesNotContain("abstract class TestController", code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_partialcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains("Foo(string test, bool? test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_abstractcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains("Foo(string test, bool? test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_operationid_then_route_attribute_name_specified()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.OperationId
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("Route(\"Foo\", Name = \"Test_Foo\")", code);
            Assert.Contains("Route(\"Bar\", Name = \"Test_Bar\")", code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_none_then_route_attribute_name_not_specified()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.None
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("Route(\"Foo\")", code);
            Assert.Contains("Route(\"Bar\")", code);
        }

        [Fact]
        public async Task When_controller_has_operations_with_required_parameters_then_partialcontroller_is_generated_with_bindrequired_attribute()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                GenerateModelValidationAttributes = true,
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains($"ComplexRequired([Microsoft.AspNetCore.Mvc.FromBody] [Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] ComplexType complexType)", code);
            Assert.Contains($"Foo(string test, bool? test2)", code);
            Assert.Contains($"FooRequired([Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] string test, [Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] bool test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_controller_has_operations_with_required_parameters_then_abstractcontroller_is_generated_with_bindrequired_attribute()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                GenerateModelValidationAttributes = true,
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains($"ComplexRequired([Microsoft.AspNetCore.Mvc.FromBody] [Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] ComplexType complexType)", code);
            Assert.Contains($"Foo(string test, bool? test2)", code);
            Assert.Contains($"FooRequired([Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] string test, [Microsoft.AspNetCore.Mvc.ModelBinding.BindRequired] bool test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_the_generation_of_dto_classes_are_disabled_then_file_is_generated_without_any_dto_clasess()
        {
            //// Arrange
            var document = await GetSwaggerDocument();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                GenerateDtoTypes = false
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.DoesNotContain("public partial class ComplexType", code);
            Assert.DoesNotContain("public partial class ComplexTypeResponse", code);
        }

        private async Task<SwaggerDocument> GetSwaggerDocument()
        {
            JsonSchema4 complexTypeSchema = new JsonSchema4();
            complexTypeSchema.Title = "ComplexType";
            complexTypeSchema.Properties["Prop1"] = new JsonProperty { Type = JsonObjectType.String, IsRequired = true };
            complexTypeSchema.Properties["Prop2"] = new JsonProperty { Type = JsonObjectType.Integer, IsRequired = true };
            complexTypeSchema.Properties["Prop3"] = new JsonProperty { Type = JsonObjectType.Boolean, IsRequired = true };
            complexTypeSchema.Properties["Prop4"] = new JsonProperty { Type = JsonObjectType.Object, Reference = complexTypeSchema, IsRequired = true };

            JsonSchema4 complexTypeReponseSchema = new JsonSchema4();
            complexTypeReponseSchema.Title = "ComplexTypeResponse";
            complexTypeReponseSchema.Properties["Prop1"] = new JsonProperty { Type = JsonObjectType.String, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop2"] = new JsonProperty { Type = JsonObjectType.Integer, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop3"] = new JsonProperty { Type = JsonObjectType.Boolean, IsRequired = true };
            complexTypeReponseSchema.Properties["Prop4"] = new JsonProperty { Type = JsonObjectType.Object, Reference = complexTypeSchema, IsRequired = true };

            var typeString = await JsonSchema4.FromTypeAsync(typeof(string));

            var document = new SwaggerDocument();
            document.Paths["Foo"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Get,
                    new SwaggerOperation {
                        OperationId = "Test_Foo",
                        Parameters = {
                            new SwaggerParameter {
                                Name = "test",
                                IsRequired = false,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.String
                            },
                            new SwaggerParameter {
                                Name = "test2",
                                IsRequired = false,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            }
                        },
                        Responses =
                        {
                            new System.Collections.Generic.KeyValuePair<string, SwaggerResponse>("200", new SwaggerResponse
                            {
                                Schema = typeString
                            })
                        }
                    }
                }
            };

            document.Paths["FooRequired"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Get,
                    new SwaggerOperation {
                        OperationId = "Test_FooRequired",
                        Parameters = {
                            new SwaggerParameter {
                                Name = "test",
                                IsRequired = true,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.String
                            },
                            new SwaggerParameter {
                                Name = "test2",
                                IsRequired = true,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            }
                        },
                        Responses =
                        {
                            new System.Collections.Generic.KeyValuePair<string, SwaggerResponse>("200", new SwaggerResponse
                            {
                                Schema = typeString
                            })
                        }
                    }
                }
            };

            document.Paths["Bar"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation {
                        OperationId = "Test_Bar",
                    }
                }
            };

            document.Paths["Complex"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation {
                        OperationId = "Test_Complex",
                        Parameters = {
                            new SwaggerParameter {
                                Name = "complexType",
                                IsRequired = false,
                                Kind = SwaggerParameterKind.Body,
                                Type = JsonObjectType.Object,
                                Reference = complexTypeSchema
                            }
                        }
                    }
                }
            };

            document.Paths["ComplexRequired"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation {
                        OperationId = "Test_ComplexRequired",
                        Parameters = {
                            new SwaggerParameter {
                                Name = "complexType",
                                IsRequired = true,
                                Kind = SwaggerParameterKind.Body,
                                Type = JsonObjectType.Object,
                                Reference = complexTypeSchema
                            }
                        },
                        Responses = {
                            new System.Collections.Generic.KeyValuePair<string, SwaggerResponse>
                            (
                                "200", new SwaggerResponse
                                {
                                    Reference = new SwaggerResponse
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
    }
}


