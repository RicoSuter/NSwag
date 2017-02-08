using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class OptionalParameterTests
    {
        public class TestController : ApiController
        {
            [Route("Test")]
            public void Test(string a, string b, string c = null)
            {
            }
        }

        [TestMethod]
        public async Task When_setting_is_enabled_then_optional_parameters_have_null_optional_value()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGenerator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();
            
            //// Assert
            Assert.IsTrue(code.Contains("TestAsync(string a, string b, string c = null)"));
            Assert.IsFalse(code.Contains("TestAsync(string a, string b, string c)"));
            Assert.IsTrue(code.Contains("TestAsync(string a, string b, string c, System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_setting_is_enabled_then_parameters_are_reordered()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<TestController>();

            //// Act
            var operation = document.Operations.First().Operation;
            var lastParameter = operation.Parameters.Last();
            operation.Parameters.Remove(lastParameter);
            operation.Parameters.Insert(0, lastParameter);
            var json = document.ToJson();

            var codeGenerator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("TestAsync(string a, string b, string c = null)"));
        }
    }
}
