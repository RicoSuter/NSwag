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
        public class ComplexType
        {
            public string Prop1 { get; set; }

            public int Prop2 { get; set; }

            public bool Prop3 { get; set; }

            public ComplexType Prop4 { get; set; }
        }

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
        public async Task When_controllergenerationformat_notsetted_then_partialcontroller_is_generated()
        {
            //// Arrange
            var document = await GetSwaggerDocument();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
            });
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
            var document = await GetSwaggerDocument();
            var json = document.ToJson();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                AspNetNamespace = "MyCustomNameSpace"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)", code);
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
                AspNetNamespace = "MyCustomNameSpace"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)", code);
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
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.None,
                RequiredAttributeType = "MyCustomType"
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
                AspNetNamespace = "MyCustomNameSpace",
                UseModelValidationAttributes = true,
                RequiredAttributeType = "MyCustomType"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)", code);
            Assert.Contains($"ComplexRequired([{settings.AspNetNamespace}.FromBody] [{settings.RequiredAttributeType}] ComplexType complexType)", code);
            Assert.Contains($"Foo(string test, bool? test2)", code);
            Assert.Contains($"FooRequired([{settings.RequiredAttributeType}] string test, [{settings.RequiredAttributeType}] bool test2)", code);
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
                UseModelValidationAttributes = true,
                AspNetNamespace = "MyCustomNameSpace",
                RequiredAttributeType = "MyCustomType"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)", code);
            Assert.Contains($"ComplexRequired([{settings.AspNetNamespace}.FromBody] [{settings.RequiredAttributeType}] ComplexType complexType)", code);
            Assert.Contains($"Foo(string test, bool? test2)", code);
            Assert.Contains($"FooRequired([{settings.RequiredAttributeType}] string test, [{settings.RequiredAttributeType}] bool test2)", code);
            Assert.Contains("Bar()", code);
        }

        private async Task<SwaggerDocument> GetSwaggerDocument()
        {
            var type = await JsonSchema4.FromTypeAsync(typeof(ComplexType));
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
                                Reference = type
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
                                Reference = type
                            }
                        }
                    }
                }
            };

            document.Definitions["ComplexType"] = type;

            return document;
        }
    }
}


