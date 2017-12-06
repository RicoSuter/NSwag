using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class AngularTests
    {
        public class Foo
        {
            public string Bar { get; set; }
        }

        public class DiscussionController : ApiController
        {

            [HttpPost]
            public void AddMessage([FromBody]Foo message)
            {
            }
        }

        [TestMethod]
        public async Task When_return_value_is_void_then_client_returns_observable_of_void()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("addMessage(message: Foo | null | undefined): Observable<void>"));
        }
    }
}
