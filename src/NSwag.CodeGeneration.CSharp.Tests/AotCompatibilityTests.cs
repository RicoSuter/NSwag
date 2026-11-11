using System.ComponentModel.DataAnnotations;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class AotCompatibilityTests
    {
        [Fact]
        public async Task When_GenerateJsonSerializerContext_is_true_then_context_class_and_AOT_safe_calls_are_emitted()
        {
            var document = CreateDocument();

            var settings = CreateAotSettings();

            var code = new CSharpClientGenerator(document, settings).GenerateFile();

            await VerifyHelper.Verify(code);

            Assert.Contains("partial class ApiJsonSerializerContext : System.Text.Json.Serialization.JsonSerializerContext", code);
            Assert.Contains("[System.Text.Json.Serialization.JsonSerializable(typeof(AotPerson), TypeInfoPropertyName = \"AotPerson\")]", code);
            Assert.Contains("settings.TypeInfoResolverChain.Insert(0, ApiJsonSerializerContext.Default);", code);
            Assert.Contains("System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> jsonTypeInfo", code);
            Assert.Contains("ApiJsonSerializerContext.Default.AotPerson, cancellationToken", code);
            Assert.DoesNotContain("JsonSerializer.Deserialize<T>(responseText, JsonSerializerSettings)", code);
            Assert.DoesNotContain("JsonSerializer.DeserializeAsync<T>(responseStream, JsonSerializerSettings", code);
        }

        [Fact]
        public async Task When_GenerateJsonSerializerContext_with_list_response_then_uses_ListOf_property_name()
        {
            var document = CreateDocumentReturningList();

            var settings = CreateAotSettings();

            var code = new CSharpClientGenerator(document, settings).GenerateFile();

            await VerifyHelper.Verify(code);

            Assert.Contains("TypeInfoPropertyName = \"ICollectionOfAotPerson\"", code);
            Assert.Contains("ApiJsonSerializerContext.Default.ICollectionOfAotPerson", code);
        }

        [Fact]
        public async Task When_GenerateJsonSerializerContext_with_form_urlencoded_then_registers_DictionaryOfStringAndString()
        {
            var document = CreateDocumentWithFormUrlEncodedBody();

            var settings = CreateAotSettings();

            var code = new CSharpClientGenerator(document, settings).GenerateFile();

            await VerifyHelper.Verify(code);

            Assert.Contains("[System.Text.Json.Serialization.JsonSerializable(typeof(System.Collections.Generic.Dictionary<string, string>), TypeInfoPropertyName = \"DictionaryOfStringAndString\")]", code);
            Assert.Contains("ApiJsonSerializerContext.Default.DictionaryOfStringAndString", code);
        }

        [Fact]
        public async Task When_GenerateJsonSerializerContext_with_multi_client_file_then_single_shared_context_is_emitted()
        {
            var document = CreateMultiControllerDocument();

            var settings = CreateAotSettings();
            settings.OperationNameGenerator = new MultipleClientsFromOperationIdOperationNameGenerator();

            var code = new CSharpClientGenerator(document, settings).GenerateFile();

            await VerifyHelper.Verify(code);

            var occurrences = CountOccurrences(code, "partial class ApiJsonSerializerContext");
            Assert.Equal(1, occurrences);
        }

        [Fact]
        public async Task When_GenerateJsonSerializerContext_with_enum_query_parameter_then_emits_generic_ConvertToString_overload()
        {
            var document = CreateDocumentWithEnumQueryParameter();

            var settings = CreateAotSettings();

            var code = new CSharpClientGenerator(document, settings).GenerateFile();

            await VerifyHelper.Verify(code);

            Assert.Contains("private string ConvertToString<[System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields)] T>(T value, System.Globalization.CultureInfo cultureInfo) where T : struct, System.Enum", code);
            Assert.Contains("UnconditionalSuppressMessage(\"Trimming\", \"IL2075\"", code);
            Assert.Contains("ConvertToString(status, System.Globalization.CultureInfo.InvariantCulture)", code);
        }

        [Fact]
        public void When_GenerateJsonSerializerContext_with_NewtonsoftJson_then_throws()
        {
            var document = CreateDocument();
            var settings = CreateAotSettings();
            settings.CSharpGeneratorSettings.JsonLibrary = CSharpJsonLibrary.NewtonsoftJson;

            var ex = Assert.Throws<InvalidOperationException>(() => new CSharpClientGenerator(document, settings));
            Assert.Contains("SystemTextJson", ex.Message);
        }

        [Fact]
        public void When_GenerateJsonSerializerContext_with_JsonLibraryVersion_below_8_then_throws()
        {
            var document = CreateDocument();
            var settings = CreateAotSettings();
            settings.CSharpGeneratorSettings.JsonLibraryVersion = 6.0m;

            var ex = Assert.Throws<InvalidOperationException>(() => new CSharpClientGenerator(document, settings));
            Assert.Contains("JsonLibraryVersion", ex.Message);
        }

        [Fact]
        public void When_GenerateJsonSerializerContext_with_NJsonSchema_polymorphism_then_throws()
        {
            var document = CreateDocument();
            var settings = CreateAotSettings();
            settings.CSharpGeneratorSettings.JsonPolymorphicSerializationStyle = CSharpJsonPolymorphicSerializationStyle.NJsonSchema;

            var ex = Assert.Throws<InvalidOperationException>(() => new CSharpClientGenerator(document, settings));
            Assert.Contains("JsonPolymorphicSerializationStyle", ex.Message);
        }

        [Fact]
        public void When_GenerateJsonSerializerContext_with_GenerateJsonMethods_then_throws()
        {
            var document = CreateDocument();
            var settings = CreateAotSettings();
            settings.CSharpGeneratorSettings.GenerateJsonMethods = true;

            var ex = Assert.Throws<InvalidOperationException>(() => new CSharpClientGenerator(document, settings));
            Assert.Contains("GenerateJsonMethods", ex.Message);
        }

        private static int CountOccurrences(string haystack, string needle)
        {
            var count = 0;
            var index = 0;
            while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += needle.Length;
            }
            return count;
        }

        private static CSharpClientGeneratorSettings CreateAotSettings()
        {
            var settings = new CSharpClientGeneratorSettings { ClassName = "AotClient" };
            settings.CSharpGeneratorSettings.Namespace = "AotNamespace";
            settings.CSharpGeneratorSettings.JsonLibrary = CSharpJsonLibrary.SystemTextJson;
            settings.CSharpGeneratorSettings.JsonLibraryVersion = 8.0m;
            settings.CSharpGeneratorSettings.JsonPolymorphicSerializationStyle = CSharpJsonPolymorphicSerializationStyle.SystemTextJson;
            settings.GenerateJsonSerializerContext = true;
            return settings;
        }

        private static OpenApiDocument CreateDocument()
        {
            var document = new OpenApiDocument();
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(schemaSettings);

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
                                    Reference = generator.Generate(typeof(AotPerson), new OpenApiSchemaResolver(document, schemaSettings))
                                }
                            }
                        }
                    }
                }
            };
            return document;
        }

        private static OpenApiDocument CreateDocumentReturningList()
        {
            var document = new OpenApiDocument();
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(schemaSettings);
            var resolver = new OpenApiSchemaResolver(document, schemaSettings);
            var personSchemaRef = generator.Generate(typeof(AotPerson), resolver);

            document.Paths["/People"] = new OpenApiPathItem
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
                                    Type = JsonObjectType.Array,
                                    Item = new JsonSchema { Reference = personSchemaRef }
                                }
                            }
                        }
                    }
                }
            };
            return document;
        }

        private static OpenApiDocument CreateDocumentWithFormUrlEncodedBody()
        {
            var document = new OpenApiDocument();
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(schemaSettings);
            var resolver = new OpenApiSchemaResolver(document, schemaSettings);
            var personSchemaRef = generator.Generate(typeof(AotPerson), resolver);

            document.Paths["/RegisterPerson"] = new OpenApiPathItem
            {
                [OpenApiOperationMethod.Post] = new OpenApiOperation
                {
                    RequestBody = new OpenApiRequestBody
                    {
                        Content =
                        {
                            ["application/x-www-form-urlencoded"] = new OpenApiMediaType
                            {
                                Schema = new JsonSchema { Reference = personSchemaRef }
                            }
                        }
                    },
                    Responses =
                    {
                        { "204", new OpenApiResponse() }
                    }
                }
            };
            return document;
        }

        private static OpenApiDocument CreateDocumentWithEnumQueryParameter()
        {
            var document = new OpenApiDocument();
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(schemaSettings);
            var resolver = new OpenApiSchemaResolver(document, schemaSettings);
            var statusSchemaRef = generator.Generate(typeof(AotStatus), resolver);

            document.Paths["/Search"] = new OpenApiPathItem
            {
                [OpenApiOperationMethod.Get] = new OpenApiOperation
                {
                    Parameters =
                    {
                        new OpenApiParameter
                        {
                            Name = "status",
                            Kind = OpenApiParameterKind.Query,
                            IsRequired = true,
                            Schema = new JsonSchema { Reference = statusSchemaRef }
                        }
                    },
                    Responses =
                    {
                        { "204", new OpenApiResponse() }
                    }
                }
            };
            return document;
        }

        private static OpenApiDocument CreateMultiControllerDocument()
        {
            var document = new OpenApiDocument();
            var schemaSettings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(schemaSettings);
            var resolver = new OpenApiSchemaResolver(document, schemaSettings);
            var personSchemaRef = generator.Generate(typeof(AotPerson), resolver);

            document.Paths["/Persons"] = new OpenApiPathItem
            {
                [OpenApiOperationMethod.Get] = new OpenApiOperation
                {
                    OperationId = "Persons_GetAll",
                    Responses = { { "200", new OpenApiResponse { Schema = new JsonSchema { Reference = personSchemaRef } } } }
                }
            };
            document.Paths["/Addresses"] = new OpenApiPathItem
            {
                [OpenApiOperationMethod.Get] = new OpenApiOperation
                {
                    OperationId = "Addresses_GetAll",
                    Responses = { { "200", new OpenApiResponse { Schema = new JsonSchema { Reference = personSchemaRef } } } }
                }
            };
            return document;
        }
    }

    public class AotPerson
    {
        [Required] public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public enum AotStatus
    {
        Pending,
        Approved,
        Rejected,
    }
}
