using System;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;

namespace NSwag.CodeGeneration.CSharp.Tests.Controllers
{
    [TestClass]
    public class ControllerGenerationDefaultParameterTests
    {
        [TestMethod]
        public void When_parameter_has_default_then_set_in_partial_controller()
        {
            //// Arrange
            var document = new SwaggerDocument();
            document.Paths["foo/bar"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Get,
                    new SwaggerOperation {
                        Parameters = {
                            new SwaggerParameter {
                                Name = "booldef",
                                IsRequired = false,
                                Default = true,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            },
                            new SwaggerParameter {
                                Name = "intdef",
                                IsRequired = false,
                                Default = 42,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new SwaggerParameter {
                                Name = "bar",
                                IsRequired = false,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new SwaggerParameter {
                                Name = "doubledef",
                                IsRequired = false,
                                Default = 0.6822871999174,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Number
                            },
                            new SwaggerParameter {
                                Name = "decdef",
                                IsRequired = false,
                                Default = 79228162514264337593543950335M,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Number,
                                Format = JsonFormatStrings.Decimal
                            },
                            new SwaggerParameter {
                                Name = "abc",
                                IsRequired = true,
                                Default = 84,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new SwaggerParameter {
                                Name = "strdef",
                                Default = @"default""string""",
                                IsRequired = false,
                                Kind = SwaggerParameterKind.Query,
                                Type = JsonObjectType.String
                            }
                        }
                    }
                }
            };

            //// Act
            var generator =
                new SwaggerToCSharpControllerGenerator(document,
                    new SwaggerToCSharpControllerGeneratorSettings { GenerateOptionalParameters = true, AspNetNamespace = "" });
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("_implementation.BarAsync(abc, booldef ?? true, intdef ?? 42, doubledef ?? 0.6822871999174D, decdef ?? 79228162514264337593543950335M, strdef ?? \"default\\\"string\\\"\", bar)"));
            Assert.IsTrue(code.Contains("BarAsync(int abc, bool booldef, int intdef, double doubledef, decimal decdef, string strdef, int? bar = null);"));

            var trimmedCode = RemoveExternalReferences(code);

            CompilerParameters parameters = new CompilerParameters { GenerateInMemory = true };

            var result = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, trimmedCode);
            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.ToString());
                }
            }

            Assert.IsTrue(result.Errors.Count == 0);
        }

        private static string RemoveExternalReferences(string code)
        {
            // Remove the external references, and verify that the generated code compiles
            var trimmedCode = code.Replace(": .ApiController", "")
                .Replace("[.HttpGet, .Route(\"foo/bar\")]", "");

            trimmedCode = new Regex(@"\[System.CodeDom.Compiler.GeneratedCode.*]").Replace(trimmedCode, "");
            return trimmedCode;
        }
    }
}