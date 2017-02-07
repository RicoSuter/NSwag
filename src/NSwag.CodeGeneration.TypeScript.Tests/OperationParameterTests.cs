using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class OperationParameterTests
    {
        public class FooController : ApiController
        {
            [Route("foos/")]
            public Foo[] GetFoos([FromUri] Bar[] bars)
            {
                return new Foo[0];
            }
        }

        public enum Bar
        {
            Baz,
            Foo
        }

        public class Foo
        {
            public Bar Bar { get; set; }

            public Bar Bar2 { get; set; }
        }

        [TestMethod]
        public async Task When_query_parameter_is_enum_array_then_the_enum_is_referenced()
        {
            //// Arrange
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}",
                DefaultEnumHandling = EnumHandling.String,
                DefaultPropertyNameHandling = PropertyNameHandling.Default,
                NullHandling = NullHandling.Swagger,
            };
            var generator = new WebApiToSwaggerGenerator(settings);

            //// Act
            var document = await generator.GenerateForControllerAsync<FooController>();
            var json = document.ToJson();

            var gen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.JQueryCallbacks
            });
            var code = gen.GenerateFile();

            //// Assert
            Assert.IsNotNull(document.Operations.First().Operation.Parameters.First().Item.SchemaReference);
            Assert.IsTrue(code.Contains("getFoos(bars: Bar[], "));
        }
    }
}
