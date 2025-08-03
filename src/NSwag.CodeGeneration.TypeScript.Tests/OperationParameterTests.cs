using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag.Generation.WebApi;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class OperationParameterTests
    {
        public class FooController : Controller
        {
            [Route("foos/")]
            public Foo[] GetFoos([FromUri] Bar[] bars)
            {
                return [];
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
        public class FromUriAttribute : Attribute;

        public enum Bar
        {
            Baz,
            Foo
        }

        public class Foo
        {
            public Bar Bar { get; set; }

            public Bar Bar2 { get; set; }
        }

        [Fact]
        public async Task When_query_parameter_is_enum_array_then_the_enum_is_referenced()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Converters = [new StringEnumConverter()]
            };

            // Arrange
            var settings = new WebApiOpenApiDocumentGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}",
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SerializerSettings = serializerSettings,
                    SchemaType = SchemaType.Swagger2
                }
            };
            var generator = new WebApiOpenApiDocumentGenerator(settings);

            // Act
            var document = await generator.GenerateForControllerAsync<FooController>();
            var json = document.ToJson();
            Assert.NotNull(json);

            var clientSettings = new TypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryCallbacks
            };

            var gen = new TypeScriptClientGenerator(document, clientSettings);
            var code = gen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            TypeScriptCompiler.AssertCompile(code);
        }
    }
}
