using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class TypeScriptOperationParameterTests
    {
        public class NullableParameterController
        {
            [Route("foo")]
            public string Test(int a, int? b)
            {
                return null;
            }
        }

        public class NullableOptionalParameterController
        {
            [Route("foo")]
            public string Test(int a, int? b = null)
            {
                return null;
            }
        }

        [Fact]
        public async Task When_parameter_is_nullable_and_ts20_then_it_is_a_union_type_with_undefined()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<NullableParameterController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_is_nullable_and_ts20_then_it_is_not_included_in_query_string()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<NullableParameterController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_is_nullable_optional_and_ts20_then_it_is_a_union_type_with_undefined()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<NullableOptionalParameterController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_parameter_is_nullable_optional_and_ts20_then_it_is_not_included_in_query_string()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<NullableOptionalParameterController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
