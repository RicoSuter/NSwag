using Microsoft.AspNetCore.Mvc;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;
using System.Collections;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class CSharpClientSettingsTests
    {
        public class FooController : Controller
        {
            public object GetPerson(bool @override = false)
            {
                return null;
            }

            public object CreatePerson(bool @override = false)
            {
                return null;
            }

#pragma warning disable S1133 // Deprecated code should be removed
            [Obsolete("Testing generation of obsolete endpoints")]
#pragma warning restore S1133 // Deprecated code should be removed
            public object DeletePerson(bool @override = false)
            {
                return null;
            }
        }

        [Fact]
        public async Task When_ConfigurationClass_is_set_then_correct_ctor_is_generated()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });
            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();

            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                InjectHttpClient = false,
                ConfigurationClass = "MyConfig",
                ClientBaseClass = "MyBaseClass"
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_UseHttpRequestMessageCreationMethod_is_set_then_CreateRequestMessage_is_generated()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                ConfigurationClass = "MyConfig",
                ClientBaseClass = "MyBaseClass",
                UseHttpRequestMessageCreationMethod = true
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task WhenUsingBaseUrl_ButNoProperty_ThenPropertyIsNotUsedAndFieldIsGenerated()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                UseBaseUrl = true,
                GenerateBaseUrlProperty = false
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_name_is_reserved_keyword_then_it_is_appended_with_at()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_code_is_generated_then_by_default_the_system_httpclient_is_used()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                InjectHttpClient = false
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_custom_http_client_type_is_specified_then_an_instance_of_that_type_is_used()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                HttpClientType = "CustomNamespace.CustomHttpClient",
                InjectHttpClient = false
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_client_base_interface_is_not_specified_then_client_interface_should_have_no_base_interface()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_client_base_interface_is_not_specified_then_client_interface_should_have_no_base_interface_and_has_correct_access_modifier()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true,
                ClientInterfaceAccessModifier = "internal"
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_client_base_interface_is_specified_then_client_interface_extends_it()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true,
                ClientBaseInterface = "IClientBase"
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_client_base_interface_is_specified_with_access_modifier_then_client_interface_extends_it_and_has_correct_access_modifier()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true,
                ClientBaseInterface = "IClientBase",
                ClientInterfaceAccessModifier = "internal"
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_client_class_generation_is_enabled_and_suppressed_then_client_class_is_not_generated()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                SuppressClientClassesOutput = true,
                GenerateClientInterfaces = true,
                // SuppressClientInterfacesOutput = false, // default
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_client_interface_generation_is_enabled_and_suppressed_then_client_interface_is_not_generated()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                // SuppressClientClassesOutput = false, // default
                GenerateClientInterfaces = true,
                SuppressClientInterfacesOutput = true,
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        public class OperationSelectionTestData : IEnumerable<object[]>
        {
            private static readonly string[] OneOperation = ["Foo_GetPerson"];
            private static readonly string[] TwoOperations = ["Foo_GetPerson", "Foo_CreatePerson"];
            private static readonly string[] ThreeOperations = ["Foo_GetPerson", "Foo_CreatePerson", "Foo_DeletePerson"];

            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] { OneOperation };
                yield return new object[] { TwoOperations };
                yield return new object[] { ThreeOperations };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        [Theory]
        [ClassData(typeof(OperationSelectionTestData))]
        public async Task When_operation_id_is_provided_in_both_included_and_excluded_operation_ids_then_throw_invalid_operation(
            string[] operationIds
            )
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                GenerateClientInterfaces = true,
                IncludedOperationIds = operationIds,
                ExcludedOperationIds = operationIds
            });

            // Act && Assert
            InvalidOperationException exception =
                Assert.Throws<InvalidOperationException>(() => generator.GenerateFile());

            Assert.Equal(
                $"Some operations are both in included and excluded operation IDs ({string.Join(", ", operationIds)}).",
                exception.Message
                );
        }

        [Theory]
        [ClassData(typeof(OperationSelectionTestData))]
        public async Task When_include_operation_ids_is_provided_then_only_selected_operations_are_included_in_generated_code(
            string[] includedOperationIds
            )
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                GenerateClientInterfaces = true,
                IncludedOperationIds = includedOperationIds
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code)
                .UseParameters(includedOperationIds.Length);
            CSharpCompiler.AssertCompile(code);
        }

        [Theory]
        [ClassData(typeof(OperationSelectionTestData))]
        public async Task When_exclude_operation_ids_is_provided_then_selected_operations_should_be_excluded_from_generated_code(
            string[] excludedOperationIds
            )
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                GenerateClientInterfaces = true,
                ExcludedOperationIds = excludedOperationIds
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code)
                .UseParameters(excludedOperationIds.Length);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_depreacted_endpoints_are_excluded_the_client_should_not_generate_these_endpoint()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                ExcludeDeprecated = true
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.DoesNotContain("DeletePerson", code);
            Assert.DoesNotContain("Obsolete", code);
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_depreacted_endpoints_are_excluded_the_client_should_still_generate_explicitly_included_endpoints()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                GenerateClientClasses = true,
                ExcludeDeprecated = true,
                IncludedOperationIds = ["Foo_DeletePerson"]
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("DeletePerson", code);
            Assert.Contains("Obsolete", code);
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }
    }
}