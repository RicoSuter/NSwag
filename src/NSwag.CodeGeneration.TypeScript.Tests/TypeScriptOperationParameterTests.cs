using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
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

        [Fact]
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
            Assert.Contains("test(a: number, b: number | null)", code);
        }
    }
}
