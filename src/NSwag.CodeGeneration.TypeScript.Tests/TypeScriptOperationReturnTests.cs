using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class TypeScriptOperationReturnTests
    {
        public class NullableReturnController
        {
            [Route("foo")]
            [return: CanBeNull]
            public string Test(int a)
            {
                return null;
            }
        }

        public class NonNullableReturnController
        {
            [Route("foo")]
            [return: NotNull]
            public string Test(int a, int? b = null)
            {
                return string.Empty;
            }
        }

        [Fact]
        public async Task When_return_value_is_nullable_and_settings_uses_null_then_it_is_a_union_type_with_null()
        {
            await Run<NullableReturnController>(TypeScriptNullValue.Null);
        }

        [Fact]
        public async Task When_return_value_is_nullable_and_settings_uses_undefined_then_it_is_a_union_type_with_undefined()
        {
            await Run<NullableReturnController>(TypeScriptNullValue.Undefined);
        }

        [Fact]
        public async Task When_return_value_is_non_nullable_and_then_it_is_not_a_union_type()
        {
            await Run<NonNullableReturnController>(TypeScriptNullValue.Null);
            await Run<NonNullableReturnController>(TypeScriptNullValue.Undefined);
        }

        private static async Task Run<TController>(TypeScriptNullValue nullSetting)
            where TController : class
        {

            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });

            var document = await generator.GenerateForControllerAsync<TController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    NullValue = nullSetting
                }
            });

            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }
    }
}
