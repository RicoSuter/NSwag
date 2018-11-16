using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

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
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                WrapResponses = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("Task<SwaggerResponse<string>>", code);
            Assert.Contains("Task<SwaggerResponse>", code);
        }

        [Fact]
        public async Task When_success_responses_are_wrapped_then_SwaggerResponse_is_returned_web_api()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                WrapResponses = true
            });
            var code = codeGen.GenerateFile();
            
            //// Assert
            Assert.Contains("Task<SwaggerResponse<string>>", code);
            Assert.Contains("Task<SwaggerResponse>", code);
        }
    }
}