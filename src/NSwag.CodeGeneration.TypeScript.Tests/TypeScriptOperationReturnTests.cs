using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;
using static NSwag.CodeGeneration.TypeScript.TypeScriptTemplate;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class TypeScriptOperationReturnTests
    {
        public class ReturnDto
        {
            public string Value { get; set; }
        }

        public class NullableReturnController
        {
            [Route("foo")]
            [return: CanBeNull]
            public ReturnDto Test(int a)
            {
                return null;
            }
        }

        public class NullableSimpleTypeReturnController
        {
            [Route("foo")]
            [return: CanBeNull]
            public string Test(int a)
            {
                return null;
            }
        }

        public class NullableReturnAnyController
        {
            [Route("foo")]
            [return: CanBeNull]
            public object Test(int a)
            {
                return null;
            }
        }

        public class NonNullableReturnAnyController
        {
            [Route("foo")]
            [return: NotNull]
            public object Test(int a, int? b = null)
            {
                return string.Empty;
            }
        }

        public class NonNullableReturnController
        {
            [Route("foo")]
            [return: NotNull]
            public ReturnDto Test(int a, int? b = null)
            {
                return new ReturnDto { Value = string.Empty };
            }
        }

        public class AllReturnKindsController
        {
            [Route("nullableString")]
            [return: CanBeNull]
            public string NullableString(int a) => null;

            [Route("nonNullableString")]
            [return: NotNull]
            public string NonNullableString(int a) => string.Empty;

            [Route("nullableObject")]
            [return: CanBeNull]
            public ReturnDto NullableObject(int a) => null;

            [Route("nonNullableObject")]
            [return: NotNull]
            public ReturnDto NonNullableObject(int a) => new ReturnDto();

            [Route("nullableAny")]
            [return: CanBeNull]
            public object NullableAny(int a) => null;

            [Route("voidReturn")]
            public void VoidReturn(int a) { }
        }


        [Fact]
        public async Task When_return_value_is_nullable_and_settings_uses_null_then_it_is_a_union_type_with_null()
        {
            await VerifyFetchTest<NullableReturnController>(TypeScriptNullValue.Null);
        }

        [Fact]
        public async Task When_return_value_is_simple_nullable_and_settings_uses_null_then_it_is_a_union_type_with_null()
        {
            await VerifyFetchTest<NullableSimpleTypeReturnController>(TypeScriptNullValue.Null);
        }

        [Fact]
        public async Task When_return_value_is_nullable_and_settings_uses_undefined_then_it_is_a_union_type_with_undefined()
        {
            await VerifyFetchTest<NullableReturnController>(TypeScriptNullValue.Undefined);
        }

        [Fact]
        public async Task When_return_value_is_simple_nullable_and_settings_uses_undefined_then_it_is_a_union_type_with_undefined()
        {
            await VerifyFetchTest<NullableSimpleTypeReturnController>(TypeScriptNullValue.Undefined);
        }

        [Fact]
        public async Task When_return_value_is_nullable_and_settings_uses_undefined_and_interfaceType_is_interface_then_it_is_a_union_type_with_undefined()
        {
            await VerifyFetchTest<NullableReturnController>(TypeScriptNullValue.Undefined, typeStyle: TypeScriptTypeStyle.Interface);
        }

        [Theory]
        [InlineData(TypeScriptNullValue.Null)]
        [InlineData(TypeScriptNullValue.Undefined)]
        public async Task When_return_value_is_non_nullable_and_then_it_is_not_a_union_type_with(TypeScriptNullValue nullSetting)
        {
            await VerifyFetchTest<NonNullableReturnController>(nullSetting);
        }

        [Theory]
        [InlineData(TypeScriptNullValue.Null)]
        [InlineData(TypeScriptNullValue.Undefined)]
        public async Task When_return_value_is_any_nullable_then_it_is_only_any(TypeScriptNullValue nullSetting)
        {
            await VerifyFetchTest<NullableReturnAnyController>(nullSetting);
        }

        [Theory]
        [InlineData(TypeScriptNullValue.Null)]
        [InlineData(TypeScriptNullValue.Undefined)]
        public async Task When_return_value_is_non_nullable_any_then_it_is_only_any(TypeScriptNullValue nullSetting)
        {
            await VerifyFetchTest<NonNullableReturnAnyController>(nullSetting);
        }

        [Theory]
        [MemberData(nameof(CompileMatrix))]
        public async Task All_return_kinds_compile_for_client_template(TypeScriptTemplate template, TypeScriptNullValue nullSetting, TypeScriptTypeStyle interfaceType)
        {
            // Arrange
            var nullValue = nullSetting == TypeScriptNullValue.Null ? "null" : "undefined";
            var innerUnion = $"{nameof(ReturnDto)} | {nullValue}";

            var code = await RunTest<AllReturnKindsController>(nullSetting, template, interfaceType);

            // The nullable object return carries the configured union type in every client kind.
            Assert.Contains(innerUnion, code);

            // Promise-based clients surface it as the awaited result type (Promise<...>, and AngularJS's
            // ng.IPromise<...> which contains that substring). JQueryCallbacks is callback-based — it returns
            // the jQuery XHR and delivers the result via an onSuccess callback — so it has no Promise wrapper.
            if (template != JQueryCallbacks)
            {
                Assert.Contains($"Promise<{innerUnion}>", code);
            }
        }

        /// <summary>Templates whose generated client can be type-checked with the test project's installed
        /// npm deps, each in both null-value modes and varying type styles. Angular (needs @angular/core + rxjs) and Aurelia (needs
        /// aurelia-fetch-client) are excluded, matching the rest of the suite (e.g. AngularTests does not
        /// compile-check its output).</summary>
        public static TheoryData<TypeScriptTemplate, TypeScriptNullValue, TypeScriptTypeStyle> CompileMatrix()
        {
            var templates = new[] { Fetch, Axios, AngularJS, JQueryCallbacks, JQueryPromises, };
            var nullValues = new[] { TypeScriptNullValue.Null, TypeScriptNullValue.Undefined };
            var interfaceTypeOptions = new[] { TypeScriptTypeStyle.Class, TypeScriptTypeStyle.Interface };
            var data = new TheoryData<TypeScriptTemplate, TypeScriptNullValue, TypeScriptTypeStyle>();

            foreach (var template in templates)
            {
                foreach (var nullValue in nullValues)
                {
                    foreach (var interfaceTypeOption in interfaceTypeOptions)
                    {
                        data.Add(template, nullValue, interfaceTypeOption);
                    }
                }
            }

            return data;
        }

        private static async Task VerifyFetchTest<TController>(TypeScriptNullValue nullSetting, TypeScriptTypeStyle typeStyle = TypeScriptTypeStyle.Class)
            where TController : class
        {
            var code = await RunTest<TController>(nullSetting, Fetch, typeStyle);
            await VerifyHelper.Verify(code);
        }

        private static async Task<string> RunTest<TController>(TypeScriptNullValue nullSetting, TypeScriptTemplate template, TypeScriptTypeStyle typeStyle)
            where TController : class
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.OpenApi3 }
            });

            var document = await generator.GenerateForControllerAsync<TController>();
            var settings = new TypeScriptClientGeneratorSettings
            {
                Template = template,
                ResponseNullValue = nullSetting,
                TypeScriptGeneratorSettings = { TypeStyle = typeStyle }
            };
            var clientGenerator = new TypeScriptClientGenerator(document, settings);


            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var code = clientGenerator.GenerateFile();

            // Assert
            TypeScriptCompiler.AssertCompile(code);
            return code;
        }
    }
}
