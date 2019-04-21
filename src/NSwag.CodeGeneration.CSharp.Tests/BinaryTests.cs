using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class BinaryTests
    {
        [Fact]
        public async Task When_body_is_binary_then_stream_is_used_as_parameter_in_CSharp()
        {
            var yaml = @"openapi: 3.0.0
servers:
  - url: https://www.example.com/
info:
  version: '2.0.0'
  title: 'Test API'   
paths:
  /files:
    post:
      tags:
        - Files
      summary: 'Add File'
      operationId: addFile
      responses:
        '200':
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/FileToken'
      requestBody:
        content:
          image/png:
            schema:
              type: string
              format: binary
components:
  schemas:
    FileToken:
      type: object
      required:
        - fileId    
      properties:  
        fileId:
          type: string
          format: uuid";

            var document = await SwaggerYamlDocument.FromYamlAsync(yaml);

            //// Act
            var codeGenerator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.Contains("public async System.Threading.Tasks.Task<FileToken> AddFileAsync(System.IO.Stream body, System.Threading.CancellationToken cancellationToken)", code);
            Assert.Contains("var content_ = new System.Net.Http.StreamContent(body);", code);
        }
    }
}
