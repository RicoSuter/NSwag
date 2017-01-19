using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
            public HttpResponseMessage DownloadFile(int id)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task When_response_is_file_then_stream_is_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<FileDownloadController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("Task<System.IO.Stream>"));
            Assert.IsTrue(code.Contains("ReadAsStreamAsync()"));
        }
    }
}
