using System.Threading.Tasks;
using Xunit;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class BinaryTests
    {
        [Fact]
        public async Task When_body_is_binary_then_blob_is_used_as_parameter_in_TypeScript()
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
            application/xml:
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
            var codeGenerator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings());
            var code = codeGenerator.GenerateFile();

            //// Assert
            Assert.Contains("addFile(body: Blob | undefined): ", code);
            Assert.Contains("\"Content-Type\": \"image/png\"", code);
            Assert.Contains("\"Accept\": \"application/xml\"", code);
            Assert.Contains("const content_ = body;", code);
        }
    }
}
