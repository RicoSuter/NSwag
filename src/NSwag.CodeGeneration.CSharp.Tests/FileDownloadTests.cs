using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
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
            var swaggerGenerator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGenerator.GenerateForControllerAsync<FileDownloadController>();

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

        [TestMethod]
        public async Task When_openapi3_contains_octet_stream_response_then_FileResponse_is_generated()
        {
            // Arrange
            var json = @"{
  ""openapi"": ""3.0.1"",
  ""paths"": {
    ""/instances/{id}/frames/{index}/raw"": {
      ""get"": {
        ""description"": ""sample"",
        ""operationId"": ""raw"",
        ""parameters"": [],
        ""responses"": {
          ""200"": {
            ""description"": ""raw file in binary data"",
            ""content"": {
              ""application/octet-stream"": {
                ""schema"": {
                  ""type"": ""string"",
                  ""type"": ""binary""
                }
              }
            }
          }
        }
      }
    }
  }
}";
            var document = await SwaggerDocument.FromJsonAsync(json);

            //// Act
            var codeGenerator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateClientInterfaces = true
            });
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("public async System.Threading.Tasks.Task<FileResponse> RawAsync("));
            Assert.IsTrue(code.Contains("var fileResponse_ = new FileResponse("));
        }
    }
}
