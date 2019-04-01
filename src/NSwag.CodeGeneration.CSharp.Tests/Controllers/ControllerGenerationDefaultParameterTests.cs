using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using NJsonSchema;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests.Controllers
{
    public class ControllerGenerationDefaultParameterTests
    {
        [Fact]
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
                    new SwaggerToCSharpControllerGeneratorSettings { GenerateOptionalParameters = true });
            var code = generator.GenerateFile();

            //// Assert
            Assert.Contains("_implementation.BarAsync(abc, booldef ?? true, intdef ?? 42, doubledef ?? 0.6822871999174D, decdef ?? 79228162514264337593543950335M, strdef ?? \"default\\\"string\\\"\", bar)", code);
            Assert.Contains("BarAsync(int abc, bool booldef, int intdef, double doubledef, decimal decdef, string strdef, int? bar = null);", code);

            var trimmedCode = RemoveExternalReferences(code);

            //CompilerParameters parameters = new CompilerParameters { GenerateInMemory = true };

            //var result = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, trimmedCode);
            //if (result.Errors.Count > 0)
            //{
            //    foreach (var error in result.Errors)
            //    {
            //        Console.WriteLine(error.ToString());
            //    }
            //}

            //Assert.True(result.Errors.Count == 0);
        }

		[Fact]
		public void When_security_schemes_mapped_to_api_then_generate_authorization_attribute_for_controller()
		{
			SwaggerDocument apiDefinition = new SwaggerDocument();

			string authenticationScheme1Name = "petstore_auth";
			string authenticationScheme2Name = "api_key";
			string authenticationScheme3Name = "Token";

			SwaggerSecurityRequirement securityRequirement1 = new SwaggerSecurityRequirement();
			securityRequirement1.Add(authenticationScheme1Name, new List<string>() { "write:pets", "read:pets" });
			securityRequirement1.Add(authenticationScheme2Name, new List<string>());

			SwaggerSecurityRequirement securityRequirement2 = new SwaggerSecurityRequirement();
			securityRequirement2.Add(authenticationScheme3Name, new List<string>());

			Collection<SwaggerSecurityRequirement> controllerSecurity = new Collection<SwaggerSecurityRequirement>()
			{ securityRequirement1, securityRequirement2 };

			apiDefinition.Security = controllerSecurity;

			apiDefinition.Paths["foo/bar"] = new SwaggerPathItem
			{
				{
					SwaggerOperationMethod.Get,
					new SwaggerOperation()
				}
			};

			SwaggerToCSharpControllerGeneratorSettings codeGeneratorSettings = new SwaggerToCSharpControllerGeneratorSettings
			{ GenerateAuthorizationAttributes = true };

			SwaggerToCSharpControllerGenerator codeGenerator = new SwaggerToCSharpControllerGenerator(apiDefinition, codeGeneratorSettings);
			string controllerCode = codeGenerator.GenerateFile();

			string expectedAuthorizationAttributeCodeTemplate =
				"    [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = \"{0},{1},{2}\")]\n" +
				"    public partial class Controller : System.Web.Http.ApiController";

			string expectedAuthorizationAttributeCode = string.Format(expectedAuthorizationAttributeCodeTemplate, authenticationScheme1Name,
				authenticationScheme2Name, authenticationScheme3Name);

			Assert.Contains(expectedAuthorizationAttributeCode, controllerCode);
		}

		[Fact]
		public void When_security_schemes_mapped_to_operation_then_generate_authorization_attribute_for_action()
		{
			SwaggerDocument apiDefinition = new SwaggerDocument();

			string authenticationScheme1Name = "petstore_auth";
			string authenticationScheme2Name = "api_key";
			string authenticationScheme3Name = "Token";

			SwaggerSecurityRequirement securityRequirement1 = new SwaggerSecurityRequirement();
			securityRequirement1.Add(authenticationScheme1Name, new List<string>() { "write:pets", "read:pets" });
			securityRequirement1.Add(authenticationScheme2Name, new List<string>());

			SwaggerSecurityRequirement securityRequirement2 = new SwaggerSecurityRequirement();
			securityRequirement2.Add(authenticationScheme3Name, new List<string>());

			Collection<SwaggerSecurityRequirement> operationSecurity = new Collection<SwaggerSecurityRequirement>()
			{ securityRequirement1, securityRequirement2 };

			apiDefinition.Paths["foo/bar"] = new SwaggerPathItem
			{
				{
					SwaggerOperationMethod.Get,
					new SwaggerOperation
					{
						Security = operationSecurity
					}
				}
			};

			SwaggerToCSharpControllerGeneratorSettings codeGeneratorSettings = new SwaggerToCSharpControllerGeneratorSettings()
			{ GenerateAuthorizationAttributes = true };

			SwaggerToCSharpControllerGenerator codeGenerator = new SwaggerToCSharpControllerGenerator(apiDefinition, codeGeneratorSettings);
			string controllerCode = codeGenerator.GenerateFile();

			string expectedAuthorizationAttributeCodeTemplate =
				"        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = \"{0},{1},{2}\")]\n" +
				"        public System.Threading.Tasks.Task Bar()";

			string expectedAuthorizationAttributeCode = string.Format(expectedAuthorizationAttributeCodeTemplate, authenticationScheme1Name,
				authenticationScheme2Name, authenticationScheme3Name);

			Assert.Contains(expectedAuthorizationAttributeCode, controllerCode);
		}

		[Fact]
		public void When_security_schemes_mapped_and_GenerateAuthorizationAttributes_not_set_then_do_not_generate_authorization_attributes()
		{
			SwaggerDocument apiDefinition = new SwaggerDocument();

			string authenticationScheme1Name = "petstore_auth";
			string authenticationScheme2Name = "api_key";
			string authenticationScheme3Name = "Token";

			SwaggerSecurityRequirement securityRequirement1 = new SwaggerSecurityRequirement();
			securityRequirement1.Add(authenticationScheme1Name, new List<string>() { "write:pets", "read:pets" });
			securityRequirement1.Add(authenticationScheme2Name, new List<string>());

			SwaggerSecurityRequirement securityRequirement2 = new SwaggerSecurityRequirement();
			securityRequirement2.Add(authenticationScheme3Name, new List<string>());

			Collection<SwaggerSecurityRequirement> securityMappings = new Collection<SwaggerSecurityRequirement>()
			{ securityRequirement1, securityRequirement2 };

			apiDefinition.Security = securityMappings;

			apiDefinition.Paths["foo/bar"] = new SwaggerPathItem
			{
				{
					SwaggerOperationMethod.Get,
					new SwaggerOperation
					{
						Security = securityMappings
					}
				}
			};

			SwaggerToCSharpControllerGeneratorSettings codeGeneratorSettings = new SwaggerToCSharpControllerGeneratorSettings();
			SwaggerToCSharpControllerGenerator codeGenerator = new SwaggerToCSharpControllerGenerator(apiDefinition, codeGeneratorSettings);
			string controllerCode = codeGenerator.GenerateFile();
			string unexpectedAuthorizationAttributeCode = "Microsoft.AspNetCore.Authorization.Authorize";
			Assert.DoesNotContain(unexpectedAuthorizationAttributeCode, controllerCode);
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