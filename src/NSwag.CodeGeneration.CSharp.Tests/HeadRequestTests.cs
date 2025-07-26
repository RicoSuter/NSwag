using Microsoft.AspNetCore.Mvc;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class HeadRequestTests
    {
        public class HeadRequestController : Controller
        {
            [HttpHead]
            public void Foo()
            {
            }
        }

        [Fact]
        public async Task When_operation_is_HTTP_head_then_no_content_is_not_used()
        {
            // Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await generator.GenerateForControllerAsync<HeadRequestController>();

            // Act
            var codeGen = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
