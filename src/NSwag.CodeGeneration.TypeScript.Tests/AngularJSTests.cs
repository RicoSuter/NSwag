using NSwag.Generation.WebApi;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class AngularJSTests
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
                Template = TypeScriptTemplate.AngularJS,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
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
                Template = TypeScriptTemplate.AngularJS,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
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
                Template = TypeScriptTemplate.AngularJS,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
        }
    }
}