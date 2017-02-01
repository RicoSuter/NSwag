using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi
{
    [TestClass]
    public class XmlTests
    {
        public class MyXmlController : ApiController
        {
            public void Post([FromBody]XmlDocument xmlDocument)
            {
            }
        }

        [TestMethod]
        public async Task When_FromBody_and_xml_document_then_consumes_is_xml()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<MyXmlController>();
            var json = document.ToJson();

            //// Assert
            var operation = document.Operations.First().Operation;
            Assert.AreEqual("application/xml", operation.Consumes[0]);
            Assert.IsNull(operation.Parameters.First().Schema);
        }

        [TestMethod]
        public async Task When_body_is_xml_then_correct_csharp_is_generated()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<MyXmlController>();

            //// Act
            var gen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = gen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("(string xmlDocument, "));
            Assert.IsTrue(code.Contains("var content_ = new System.Net.Http.StringContent(xmlDocument);"));
            Assert.IsTrue(code.Contains("content_.Headers.ContentType.MediaType = \"application/xml\";"));
        }

        [TestMethod]
        public async Task When_body_is_xml_then_correct_TypeScript_is_generated()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<MyXmlController>();

            //// Act
            var gen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings { Template = TypeScriptTemplate.JQueryCallbacks });
            var code = gen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("(xmlDocument: string, "));
            Assert.IsTrue(code.Contains("const content_ = xmlDocument;"));
            Assert.IsTrue(code.Contains("\"Content-Type\": \"application/xml; charset=UTF-8\""));
        }
    }
}
