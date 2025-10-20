using Microsoft.AspNetCore.Mvc;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class WrapResponsesTests
    {
        public class TestController : Controller
        {
            [Route("Foo")]
            public string Foo()
            {
                throw new NotImplementedException();
            }

            [Route("Bar")]
            public void Bar()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public async Task When_success_responses_are_wrapped_then_SwaggerResponse_is_returned()
        {
            // Arrange
            var swaggerGen = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            // Act
            var codeGen = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
            {
                WrapResponses = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_success_responses_are_wrapped_then_SwaggerResponse_is_returned_web_api()
        {
            // Arrange
            var swaggerGen = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                WrapResponses = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
namespace MyNamespace
{
    public static class Extensions
    {
        public static string[] ToArray(this object strings) { return new string[0]; }
    }
}
");
        }

        [Fact]
        public async Task When_success_responses_are_wrapped_then_SwaggerResponse_is_returned_web_api_aspnetcore()
        {
            // Arrange
            var swaggerGen = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                IsAspNetCore = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            // Act
            var codeGen = new CSharpControllerGenerator(document, new CSharpControllerGeneratorSettings
            {
                WrapResponses = true,
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CSharpCompiler.AssertCompile(code + @"
namespace MyNamespace
{
    public static class Extensions
    {
        public static string[] ToArray(this object strings) { return new string[0]; }
    }
}
");
        }
    }
}