using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.ClientGeneration.CSharp
{
    [TestClass]
    public class FileDownloadTests
    {
        public class FileDownloadController : ApiController
        {
            [Route("DownloadFile")]
            public HttpResponseMessage DownloadFile()
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task When_response_is_file_and_stream_is_not_used_then_byte_array_is_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<FileDownloadController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task<FileResponse> DownloadFileAsync();"));
            Assert.IsTrue(code.Contains("ReadAsStreamAsync()"));
        }
    }
}
