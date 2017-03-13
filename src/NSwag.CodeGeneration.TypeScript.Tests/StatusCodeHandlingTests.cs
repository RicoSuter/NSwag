using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.Annotations;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    [TestClass]
    public class StatusCodeHandlingTests
    {
        public class FooController : ApiController
        {
            [Route("foos/")]
            [SwaggerResponse(200, null)]
            [SwaggerResponse(201, null)]
            [SwaggerResponse(301, null)]
            [SwaggerResponse(400, null)]
            [SwaggerResponse(500, null)]
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
        public async Task When_multiple_success_and_error_response_codes_are_declared_typescript_exceptions_are_only_thrown_for_error_codes()
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

            var gen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                Template = TypeScriptTemplate.Angular2
            });

            var code = gen.GenerateFile();

            var codeOnASingleLine = string.Join("",
                code.Split(new[] { Environment.NewLine, "  " }, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim()).Where(n => n.Length > 0));

            Assert.IsFalse(codeOnASingleLine.Contains("} else if (status === 201) {this.throwException(\"A server error occurred.\", status, responseText);"));
            Assert.IsTrue(codeOnASingleLine.Contains("} else if (status === 201) {return null;"));
        }
    }
}
