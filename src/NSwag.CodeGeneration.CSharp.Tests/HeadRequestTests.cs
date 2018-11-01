using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

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
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<HeadRequestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.DoesNotContain("System.Net.Http.StringContent", code);
        }
    }
}
