using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class JQueryPromisesTests
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

        [Fact]
        public async Task When_export_types_is_true_then_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryPromises,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = true
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.True(code.Contains("export class DiscussionClient"));
            Assert.True(code.Contains("export interface IDiscussionClient"));
        }

        [Fact]
        public async Task When_export_types_is_false_then_dont_add_export_before_classes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<DiscussionController>();
            var json = document.ToJson();

            //// Act
            var codeGen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryPromises,
                GenerateClientInterfaces = true,
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m,
                    ExportTypes = false
                }
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.False(code.Contains("export class DiscussionClient"));
            Assert.False(code.Contains("export interface IDiscussionClient"));
        }
    }
}
