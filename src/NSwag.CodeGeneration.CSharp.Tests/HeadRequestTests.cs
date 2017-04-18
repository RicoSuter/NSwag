using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class HeadRequestTests
    {
        public class HeadRequestController : ApiController
        {
            [HttpHead]
            public void Foo()
            {
            }
        }

        [TestMethod]
        public async Task When_operation_is_HTTP_head_then_no_content_is_not_used()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<HeadRequestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsFalse(code.Contains("System.Net.Http.StringContent"));
        }
    }
}
