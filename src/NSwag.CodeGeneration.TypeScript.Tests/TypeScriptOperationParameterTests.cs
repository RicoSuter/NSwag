using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class TypeScriptOperationParameterTests
    {
        public class OptionalParameterController
        {
            [Route("foo")]
            public string Test(int a, int? b)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_parameter_is_nullable_and_ts20_then_it_is_a_union_type_with_undefined()
        {
            //// Arrange
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<OptionalParameterController>();
            var clientGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                TypeScriptGeneratorSettings =
                {
                    TypeScriptVersion = 2.0m, 
                    NullValue = TypeScriptNullValue.Undefined
                }
            });

            var json = document.ToJson();

            //// Act
            var code = clientGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("test(a: number, b: number | null)"));
        }
    }
}
