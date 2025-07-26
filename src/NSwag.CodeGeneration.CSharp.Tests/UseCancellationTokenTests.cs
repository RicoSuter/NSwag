using Microsoft.AspNetCore.Mvc;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.CodeGeneration.Tests;
using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class UseCancellationTokenTests
    {
        public class TestController : Controller
        {
            [Route("Foo")]
            public string Foo(string test, bool test2)
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
        public async Task When_controllerstyleispartial_and_usecancellationtokenistrue_and_requesthasnoparameter_then_cancellationtoken_is_added()
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
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllerstyleispartial_and_usecancellationtokenistrue_and_requesthasparameter_then_cancellationtoken_is_added()
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
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllerstyleisabstract_and_usecancellationtokenistrue_and_requesthasnoparameter_then_cancellationtoken_is_added()
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
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_controllerstyleisabstract_and_usecancellationtokenistrue_and_requesthasparameter_then_cancellationtoken_is_added()
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
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }

        [Fact]
        public async Task When_usecancellationtokenparameter_notsetted_then_cancellationtoken_isnot_added()
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
                ControllerStyle = CSharpControllerStyle.Abstract
            });
            var code = codeGen.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}
