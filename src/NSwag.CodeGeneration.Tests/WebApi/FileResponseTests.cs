using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi
{
    [TestClass]
    public class FileResponseTests
    {
        public class FileResponseController : ApiController
        {
            public IHttpActionResult GetFile(string fileName)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task When_response_is_file_then_mime_type_is_bytes()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());
            
            //// Act
            var document = await generator.GenerateForControllerAsync<FileResponseController>();
            var json = document.ToJson();

            //// Assert
            var operation = document.Operations.First().Operation;

            //Assert.AreEqual("application/octet-stream", operation.ActualProduces.First());
            Assert.AreEqual(JsonObjectType.File, operation.Responses.First().Value.Schema.Type);
            
            // TODO: File response should produce application/octet-stream
        }
    }
}