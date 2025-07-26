using System.Text.RegularExpressions;
using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests.Controllers
{
    public class ControllerGenerationDefaultParameterTests
    {
        [Fact]
        public async Task When_parameter_has_default_then_set_in_partial_controller()
        {
            // Arrange
            var document = new OpenApiDocument();
            document.Paths["foo/bar"] = new OpenApiPathItem
            {
                {
                    OpenApiOperationMethod.Get,
                    new OpenApiOperation {
                        Parameters = {
                            new OpenApiParameter {
                                Name = "booldef",
                                IsRequired = false,
                                Default = true,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Boolean
                            },
                            new OpenApiParameter {
                                Name = "intdef",
                                IsRequired = false,
                                Default = 42,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new OpenApiParameter {
                                Name = "bar",
                                IsRequired = false,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new OpenApiParameter {
                                Name = "doubledef",
                                IsRequired = false,
                                Default = 0.6822871999174,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Number
                            },
                            new OpenApiParameter {
                                Name = "decdef",
                                IsRequired = false,
                                Default = 79228162514264337593543950335M,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Number,
                                Format = JsonFormatStrings.Decimal
                            },
                            new OpenApiParameter {
                                Name = "abc",
                                IsRequired = true,
                                Default = 84,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.Integer
                            },
                            new OpenApiParameter {
                                Name = "strdef",
                                Default = @"default""string""",
                                IsRequired = false,
                                Kind = OpenApiParameterKind.Query,
                                Type = JsonObjectType.String
                            }
                        }
                    }
                }
            };

            // Act
            var generator =
                new CSharpControllerGenerator(document,
                    new CSharpControllerGeneratorSettings { GenerateOptionalParameters = true });
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
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