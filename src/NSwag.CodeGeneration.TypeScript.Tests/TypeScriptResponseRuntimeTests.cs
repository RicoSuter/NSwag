using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.Annotations;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    /// <summary>
    /// Verifies the *runtime* value a generated client resolves to for the "nothing" cases (JSON null
    /// body, empty body, HTTP 204), and that it agrees with the declared return type as controlled by
    /// <see cref="TypeScriptClientGeneratorSettings.ResponseNullValue"/>.
    ///
    /// Empty-body semantics follow the "value, not absent" decision: the null token applies only where
    /// the value is genuinely null/absent. A nullable DTO funnels all three cases to null uniformly
    /// across templates; a nullable string is additionally covered for the JSON-null and 204 cases (an
    /// empty body for an Axios primitive stays "" and is intentionally not asserted here).
    /// </summary>
    public class TypeScriptResponseRuntimeTests
    {
        private const string UndefinedResult = "undefined";
        private const string NullResult = "null";

        public class ReturnDto
        {
            public string Value { get; set; }
        }

        public class NullableObjectReturnController
        {
            [Route("foo")]
            [return: CanBeNull]
            public ReturnDto Test(int a)
            {
                return null;
            }
        }

        public class NullableStringReturnController
        {
            [Route("foo")]
            [return: CanBeNull]
            public string Test(int a)
            {
                return null;
            }
        }

        public enum ResponseBodyCase
        {
            /// <summary>HTTP 200 whose body is the literal JSON token <c>null</c>.</summary>
            JsonNull,

            /// <summary>HTTP 200 with an empty body (no bytes).</summary>
            EmptyBody,

            /// <summary>HTTP 204 No Content.</summary>
            NoContent
        }

        // --- Nullable DTO return: exercised for both DTO type styles (class = fromJS funnel, interface =
        // JSON.parse cast) so both conversion paths are covered at runtime. ---

        /// <summary>Every (type style, template, body case) combination that resolves to the configured null
        /// value. An empty body for an <c>interface</c> DTO on Axios stays <c>""</c> (a value, not absent —
        /// no fromJS funnel), so that one combination is excluded, mirroring the primitive string case.</summary>
        public static TheoryData<TypeScriptTypeStyle, TypeScriptTemplate, ResponseBodyCase> DtoMatrix()
        {
            var bodyCases = Enum.GetValues<ResponseBodyCase>();
            var typeStyles = new[] { TypeScriptTypeStyle.Class, TypeScriptTypeStyle.Interface };
            var templateKinds = new[] { TypeScriptTemplate.Fetch, TypeScriptTemplate.Axios };
            var data = new TheoryData<TypeScriptTypeStyle, TypeScriptTemplate, ResponseBodyCase>();
            foreach (var typeStyle in typeStyles)
            {
                foreach (var template in templateKinds)
                {
                    foreach (var bodyCase in bodyCases)
                    {
                        if (typeStyle == TypeScriptTypeStyle.Interface
                            && template == TypeScriptTemplate.Axios
                            && bodyCase == ResponseBodyCase.EmptyBody)
                        {
                            continue;
                        }

                        data.Add(typeStyle, template, bodyCase);
                    }
                }
            }

            return data;
        }

        [Theory]
        [MemberData(nameof(DtoMatrix))]
        public async Task Dto_when_ResponseNullValue_is_Undefined_then_runtime_resolves_to_undefined(
            TypeScriptTypeStyle typeStyle, TypeScriptTemplate template, ResponseBodyCase bodyCase)
        {
            var result = await RunClient<NullableObjectReturnController>(
                template, TypeScriptNullValue.Undefined, bodyCase, typeStyle);
            Assert.Equal(UndefinedResult, result);
        }

        [Theory]
        [MemberData(nameof(DtoMatrix))]
        public async Task Dto_when_ResponseNullValue_is_Null_then_runtime_resolves_to_null(
            TypeScriptTypeStyle typeStyle, TypeScriptTemplate template, ResponseBodyCase bodyCase)
        {
            var result = await RunClient<NullableObjectReturnController>(
                template, TypeScriptNullValue.Null, bodyCase, typeStyle);
            Assert.Equal(NullResult, result);
        }

        // --- Nullable string return: the maintainer's example. JSON-null and 204 are well-defined on both. ---

        [Theory]
        [InlineData(TypeScriptTemplate.Fetch, ResponseBodyCase.JsonNull)]
        [InlineData(TypeScriptTemplate.Fetch, ResponseBodyCase.NoContent)]
        [InlineData(TypeScriptTemplate.Axios, ResponseBodyCase.JsonNull)]
        [InlineData(TypeScriptTemplate.Axios, ResponseBodyCase.NoContent)]
        public async Task String_when_ResponseNullValue_is_Undefined_then_runtime_resolves_to_undefined(
            TypeScriptTemplate template, ResponseBodyCase bodyCase)
        {
            var result = await RunClient<NullableStringReturnController>(
                template, TypeScriptNullValue.Undefined, bodyCase);
            Assert.Equal(UndefinedResult, result);
        }

        [Theory]
        [InlineData(TypeScriptTemplate.Fetch, ResponseBodyCase.JsonNull)]
        [InlineData(TypeScriptTemplate.Fetch, ResponseBodyCase.NoContent)]
        [InlineData(TypeScriptTemplate.Axios, ResponseBodyCase.JsonNull)]
        [InlineData(TypeScriptTemplate.Axios, ResponseBodyCase.NoContent)]
        public async Task String_when_ResponseNullValue_is_Null_then_runtime_resolves_to_null(
            TypeScriptTemplate template, ResponseBodyCase bodyCase)
        {
            var result = await RunClient<NullableStringReturnController>(
                template, TypeScriptNullValue.Null, bodyCase);
            Assert.Equal(NullResult, result);
        }

        private static async Task<string> RunClient<TController>(
            TypeScriptTemplate template, TypeScriptNullValue responseNullValue, ResponseBodyCase bodyCase,
            TypeScriptTypeStyle typeStyle = TypeScriptTypeStyle.Class)
            where TController : class
        {
            // The generated client class name mirrors the controller name with "Controller" -> "Client".
            var clientClassName = typeof(TController).Name.Replace("Controller", "") + "Client";

            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.OpenApi3 }
            });

            var document = await generator.GenerateForControllerAsync<TController>();
            var clientGenerator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = template,
                ResponseNullValue = responseNullValue,
                TypeScriptGeneratorSettings = { TypeStyle = typeStyle }
            });

            var code = clientGenerator.GenerateFile() + BuildHarness(clientClassName, bodyCase);

            // Act
            var output = TypeScriptRunner.Run(code);

            // Assert (return the parsed runtime kind so the caller can assert on it)
            const string marker = "__RUNTIME_RESULT__=";
            var line = output
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(l => l.StartsWith(marker, StringComparison.Ordinal));

            Assert.NotNull(line);
            return line.Substring(marker.Length).Trim();
        }

        /// <summary>
        /// Appends a harness that drives the generated client's public method through a mocked transport.
        /// The single fake response satisfies both the Fetch shape (<c>status</c>/<c>headers.forEach</c>/
        /// <c>text()</c>) and the Axios shape (<c>status</c>/<c>headers</c>/<c>data</c>), so the same
        /// harness works for both templates.
        /// </summary>
        private static string BuildHarness(string clientClassName, ResponseBodyCase bodyCase)
        {
            var (status, text, data) = bodyCase switch
            {
                ResponseBodyCase.JsonNull => ("200", "\"null\"", "null"),
                ResponseBodyCase.EmptyBody => ("200", "\"\"", "\"\""),
                ResponseBodyCase.NoContent => ("204", "\"\"", "undefined"),
                _ => throw new ArgumentOutOfRangeException(nameof(bodyCase))
            };

            return $$"""


                // === runtime harness (appended by test) ===
                (async () => {
                    const fakeResponse: any = {
                        status: {{status}},
                        headers: { forEach: (_cb: any) => { } },
                        text: () => Promise.resolve({{text}}),
                        data: {{data}},
                    };
                    const transport: any = {
                        fetch: (_u: any, _i: any) => Promise.resolve(fakeResponse),
                        request: (_o: any) => Promise.resolve(fakeResponse),
                    };
                    const client = new {{clientClassName}}("http://localhost", transport);
                    const result = await client.test(1);
                    const kind = result === undefined ? "{{UndefinedResult}}" : (result === null ? "{{NullResult}}" : typeof result);
                    console.log("__RUNTIME_RESULT__=" + kind);
                })();
                """;
        }
    }
}
