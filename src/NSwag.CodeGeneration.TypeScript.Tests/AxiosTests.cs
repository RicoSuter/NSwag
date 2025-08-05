using NSwag.Generation.WebApi;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class AxiosTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        public class DiscussionController : Controller
        {
            [HttpPost]
            public void AddMessage([FromBody] Foo message)
            {
            }
        }

        public class UrlEncodedRequestConsumingController : Controller
        {
            [HttpPost]
            [Consumes("application/x-www-form-urlencoded")]
            public void AddMessage([FromForm] Foo message, [FromForm] string messageId)
            {
            }

            [HttpGet]
            public Foo GetMessage([FromQuery] string messageId)
            {
                return new Foo { Bar = $"Hello World ({messageId})" };
            }
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
                Template = TypeScriptTemplate.Axios,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
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
                Template = TypeScriptTemplate.Axios,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
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
                Template = TypeScriptTemplate.Axios
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task Add_cancel_token_to_every_call()
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
                Template = TypeScriptTemplate.Axios,
                UseAbortSignal = false
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_abort_signal()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Axios,
                UseAbortSignal = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_abort_signal_and_generate_client_interfaces_contains_signal_param_in_both_interface_and_concrete_implementation()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            });

            var document = await generator.GenerateForControllerAsync<UrlEncodedRequestConsumingController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            // Act
            var codeGen = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Axios,
                UseAbortSignal = true,
                GenerateClientInterfaces = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }
    }
}
