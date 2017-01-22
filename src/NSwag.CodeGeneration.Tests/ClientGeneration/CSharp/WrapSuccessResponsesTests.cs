using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.ClientGeneration.CSharp
{
    [TestClass]
    public class WrapSuccessResponsesTests
    {
        public class TestController : ApiController
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

        [TestMethod]
        public async Task When_success_responses_are_wrapped_then_SwaggerResponse_is_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                WrapSuccessResponses = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("Task<SwaggerResponse<string>>"));
            Assert.IsTrue(code.Contains("Task<SwaggerResponse>"));
        }
    }
}