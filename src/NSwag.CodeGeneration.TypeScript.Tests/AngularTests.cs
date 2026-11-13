using NSwag.Generation.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class AngularTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        [Route("[controller]/[action]")]
        public class DiscussionController : Controller
        {
            [HttpPost]
            public void AddMessage([FromBody, Required] Foo message)
            {
            }

            [HttpPost]
            public void GenericRequestTest1(GenericRequest1 request)
            {

            }

            [HttpPost]
            public void GenericRequestTest2(GenericRequest2 request)
            {

            }
        }

        public class GenericRequestBase<T>
            where T : RequestBodyBase
        {
            [Required]
            public T Request { get; set; }
        }

        public class RequestBodyBase
        {

        }

        public class RequestBody : RequestBodyBase
        {

        }

        public class GenericRequest1 : GenericRequestBase<RequestBodyBase>
        {

        }

        public class GenericRequest2 : GenericRequestBase<RequestBody>
        {

        }

        public class UrlEncodedRequestConsumingController : Controller
        {
            [HttpPost]
            [Consumes("application/x-www-form-urlencoded")]
            public void AddMessage([FromForm] Foo message, [FromForm] string messageId)
            {
            }
        }

        [Fact]
        public async Task When_return_value_is_void_then_client_returns_observable_of_void()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_export_types_is_true_then_add_export_before_classes()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m,
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_export_types_is_false_then_dont_add_export_before_classes()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_generic_request()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateDtoTypes = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_consumes_is_url_encoded_then_construct_url_encoded_request()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }

        [Route("[controller]/[action]")]
        public class ResourceTestController : Controller
        {
            [HttpGet]
            [return: NJsonSchema.Annotations.NotNull]
            public Foo GetMessage()
            {
                return new Foo();
            }

            [HttpPost]
            public void AddMessage([FromBody, Required] Foo message)
            {
            }
        }

        [Fact]
        public async Task When_resources_are_disabled_then_existing_output_is_unchanged()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<ResourceTestController>();

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 4.3m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("Observable<Foo>", code);
            Assert.Contains("Observable<void>", code);
            Assert.DoesNotContain("getMessageResource(", code);
            Assert.DoesNotContain("addMessageResource(", code);
            Assert.DoesNotContain("rxResource", code);
            Assert.DoesNotContain("ResourceRef", code);
            Assert.DoesNotContain("BaseResourceOptions", code);
        }

        [Route("[controller]/[action]")]
        public class ResourceOperationsController : Controller
        {
            [HttpGet]
            [return: NJsonSchema.Annotations.NotNull]
            public Foo GetMessage()
            {
                return new Foo();
            }

            [HttpPost]
            public void AddMessage([FromBody, Required] Foo message)
            {
            }

            [HttpPut]
            public void UpdateMessage([FromBody, Required] Foo message)
            {
            }

            [HttpPatch]
            public void PatchMessage([FromBody, Required] Foo message)
            {
            }

            [HttpDelete]
            public void DeleteMessage()
            {
            }

            [HttpHead]
            public void HeadMessage()
            {
            }

            [HttpPost]
            [Consumes("application/x-www-form-urlencoded")]
            public void SubmitForm([FromForm, Required] string message)
            {
            }

            [HttpPost]
            [Consumes("multipart/form-data")]
            public void UploadFile([FromForm, Required] Microsoft.AspNetCore.Http.IFormFile file)
            {
            }
        }

        [Fact]
        public async Task When_resources_are_enabled_then_all_operations_have_companions()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings { SchemaType = SchemaType.Swagger2 }
            });
            var document = await generator.GenerateForControllerAsync<ResourceOperationsController>();
            var settings = new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 5.9m
                }
            };
            settings.GenerateAngularResourceMethods = true;

            // Act
            var codeGen = new TypeScriptClientGenerator(document, settings);
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("import { rxResource } from '@angular/core/rxjs-interop';", code);
            Assert.Contains("ResourceRef<Foo | undefined>", code);
            Assert.Contains("ResourceRef<void | undefined>", code);
            foreach (var (name, arguments) in new[]
            {
                ("getMessage", ""),
                ("addMessage", "params.message"),
                ("updateMessage", "params.message"),
                ("patchMessage", "params.message"),
                ("deleteMessage", ""),
                ("headMessage", ""),
                ("submitForm", "params.message"),
                ("uploadFile", "params.file")
            })
            {
                Assert.Matches($@"(?m)^    {name}\(", code);
                Assert.Contains($"{name}Resource(options", code);
                Assert.Contains($"this.{name}({arguments})", code);
            }

            await VerifyHelper.Verify(code);
        }

        [Fact]
        public async Task When_parameters_are_generated_then_resource_delegation_preserves_the_contract()
        {
            // Arrange
            const string specification = """
                                         {
                                           "openapi": "3.0.0",
                                           "paths": {
                                             "/messages/{id}": {
                                               "get": {
                                                 "operationId": "GetMessage",
                                                 "parameters": [
                                                   {
                                                     "name": "filter", "in": "query",
                                                     "schema": { "type": "string" }
                                                   },
                                                   {
                                                     "name": "id", "in": "path", "required": true,
                                                     "schema": { "type": "integer" }
                                                   },
                                                   {
                                                     "name": "revision", "in": "query", "required": true,
                                                     "schema": { "type": "integer", "nullable": true }
                                                   },
                                                   {
                                                     "name": "tenant", "in": "header", "required": true,
                                                     "schema": { "type": "string" }
                                                   }
                                                 ],
                                                 "responses": {
                                                   "200": {
                                                     "description": "Message",
                                                     "content": {
                                                       "application/json": { "schema": { "type": "string" } }
                                                     }
                                                   }
                                                 }
                                               }
                                             }
                                           }
                                         }
                                         """;
            var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                ClassName = "ParameterClient",
                GenerateAngularResourceMethods = true,
                GenerateClientInterfaces = true,
                GenerateOptionalParameters = true,
                IncludeHttpContext = true,
                InjectionTokenType = InjectionTokenType.InjectionToken,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 5.9m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            Assert.Contains("interface IParameterClient", code);
            Assert.Contains("id: number;", code);
            Assert.Contains("revision: number | null;", code);
            Assert.Contains("tenant: string;", code);
            Assert.Contains("filter?: string | undefined;", code);
            Assert.Contains("httpContext?: HttpContext;", code);
            Assert.Contains("this.getMessage(params.id, params.revision, params.tenant, params.filter, params.httpContext)", code);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task When_parameterless_get_is_generated_then_options_are_optional(bool includeHttpContext)
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<ResourceTestController>();

            // Act
            var code = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateAngularResourceMethods = true,
                GenerateClientInterfaces = true,
                IncludeHttpContext = includeHttpContext,
                InjectionTokenType = InjectionTokenType.InjectionToken
            }).GenerateFile();

            // Assert
            Assert.Contains("getMessageResource(options?: BaseResourceOptions<", code);
            Assert.Contains(includeHttpContext ? "this.getMessage(params?.httpContext)" : "this.getMessage()", code);
        }

        [Fact]
        public async Task When_resource_options_are_generated_then_native_options_are_preserved()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<ResourceTestController>();

            // Act
            var code = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateAngularResourceMethods = true,
                GenerateClientInterfaces = true,
                InjectionTokenType = InjectionTokenType.InjectionToken
            }).GenerateFile();

            // Assert
            Assert.Contains("type AngularResourceOptions<T, P> = BaseResourceOptions<T, P | undefined>", code);
            Assert.Contains("Required<Pick<BaseResourceOptions<T, P | undefined>, \"params\">>", code);
            Assert.Contains("...options,", code);
            Assert.Contains("getMessageResource(options?: BaseResourceOptions<Foo, {} | undefined>): ResourceRef<Foo | undefined>", code);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task When_result_shapes_vary_then_resources_keep_original_result_types(bool wrapResponses, bool useHooks)
        {
            // Arrange
            const string specification = """
                {
                  "openapi": "3.0.0",
                  "paths": {
                    "/dto": { "get": { "operationId": "GetDto", "responses": {
                      "200": { "description": "DTO", "content": { "application/json": { "schema": { "$ref": "#/components/schemas/Item" } } } }
                    } } },
                    "/nullable": { "get": { "operationId": "GetNullable", "responses": {
                      "200": { "description": "Nullable", "content": { "application/json": { "schema": { "type": "string", "nullable": true } } } }
                    } } },
                    "/empty": { "get": { "operationId": "GetEmpty", "responses": { "204": { "description": "Empty" } } } },
                    "/download": { "get": { "operationId": "Download", "responses": {
                      "200": { "description": "File", "content": { "application/octet-stream": { "schema": { "type": "string", "format": "binary" } } } }
                    } } }
                  },
                  "components": { "schemas": { "Item": {
                    "type": "object", "required": ["timestamp"],
                    "properties": { "timestamp": { "type": "string", "format": "date-time" } }
                  } } }
                }
                """;
            var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);

            // Act
            var code = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateAngularResourceMethods = true,
                InjectionTokenType = InjectionTokenType.InjectionToken,
                WrapResponses = wrapResponses,
                ResponseClass = "ApiResponse",
                ClientBaseClass = useHooks ? "ResourceBase" : null,
                UseTransformOptionsMethod = useHooks,
                UseTransformResultMethod = useHooks
            }).GenerateFile();

            // Assert
            foreach (var (name, resultType) in new[]
            {
                ("getDto", "Item"), ("getNullable", "string | null"), ("getEmpty", "void"), ("download", "FileResponse")
            })
            {
                var type = wrapResponses ? $"ApiResponse<{resultType}>" : resultType;
                Assert.Contains($"ResourceRef<{type} | undefined>", code);
                Assert.Contains($"this.{name}()", code);
            }
            Assert.Contains("new Date(", code);
        }

        [Theory]
        [InlineData(TypeScriptTemplate.Fetch, HttpClass.HttpClient, 6, "Template")]
        [InlineData(TypeScriptTemplate.Angular, HttpClass.Http, 6, "HttpClass")]
        [InlineData(TypeScriptTemplate.Angular, HttpClass.HttpClient, 5, "RxJsVersion")]
        public void When_resource_settings_are_unsupported_then_generation_explains_the_error(TypeScriptTemplate template, HttpClass httpClass, int rxJsVersion, string invalidSetting)
        {
            // Arrange
            var settings = new TypeScriptClientGeneratorSettings
            {
                GenerateAngularResourceMethods = true,
                Template = template,
                HttpClass = httpClass,
                RxJsVersion = rxJsVersion
            };

            // Act
            var exception = Assert.Throws<ArgumentException>(() => new TypeScriptClientGenerator(new OpenApiDocument(), settings).GenerateFile());

            // Assert
            Assert.Contains(invalidSetting, exception.Message);
            Assert.Contains("GenerateAngularResourceMethods", exception.Message);
        }

        [Fact]
        public async Task When_companion_name_conflicts_then_generation_fails_clearly()
        {
            // Arrange
            const string specification = """
                { "openapi": "3.0.0", "paths": {
                  "/user": { "get": { "operationId": "Users_GetUser", "responses": { "204": { "description": "Empty" } } } },
                  "/resource": { "get": { "operationId": "Users_GetUserResource", "responses": { "204": { "description": "Empty" } } } }
                } }
                """;
            var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
            var settings = new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateAngularResourceMethods = true
            };

            // Act
            var exception = Assert.Throws<InvalidOperationException>(() => new TypeScriptClientGenerator(document, settings).GenerateFile());

            // Assert
            Assert.Contains("UsersClient", exception.Message);
            Assert.Contains("getUserResource", exception.Message);
            settings.GenerateAngularResourceMethods = false;
            var code = new TypeScriptClientGenerator(document, settings).GenerateFile();
            Assert.Contains("getUser(): Observable<void>", code);
            Assert.Contains("getUserResource(): Observable<void>", code);
        }

        [Theory]
        [InlineData(true, false, true)]
        [InlineData(false, false, true)]
        [InlineData(true, true, true)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        public async Task When_export_and_visibility_settings_vary_then_companions_follow_existing_conventions(bool exportTypes, bool protectedMethod, bool importRequiredTypes)
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<ResourceTestController>();

            // Act
            var code = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateAngularResourceMethods = true,
                GenerateClientInterfaces = true,
                ImportRequiredTypes = importRequiredTypes,
                ProtectedMethods = protectedMethod ? ["ResourceTestClient.getMessage"] : [],
                InjectionTokenType = InjectionTokenType.InjectionToken,
                TypeScriptGeneratorSettings = { ExportTypes = exportTypes }
            }).GenerateFile();

            // Assert
            Assert.Equal(exportTypes, code.Contains("export class ResourceTestClient", StringComparison.Ordinal));
            Assert.Equal(exportTypes, code.Contains("export interface IResourceTestClient", StringComparison.Ordinal));
            Assert.Equal(importRequiredTypes, code.Contains("import { rxResource }", StringComparison.Ordinal));
            if (protectedMethod)
            {
                Assert.Contains("protected getMessageResource(", code);
                var interfaceCode = code.Substring(code.IndexOf("interface IResourceTestClient", StringComparison.Ordinal));
                interfaceCode = interfaceCode.Substring(0, interfaceCode.IndexOf("@Injectable", StringComparison.Ordinal));
                Assert.DoesNotContain("getMessageResource", interfaceCode);
            }
        }
    }
}
