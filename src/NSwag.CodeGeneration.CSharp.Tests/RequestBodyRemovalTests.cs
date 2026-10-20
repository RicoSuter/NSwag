namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class RequestBodyRemovalTests
    {
        [Fact]
        public async Task When_request_body_is_cleared_then_generated_client_has_no_body_argument()
        {
            var document = await OpenApiDocument.FromJsonAsync("""
                {
                  "openapi": "3.0.0",
                  "info": { "title": "Body removal", "version": "1.0" },
                  "paths": {
                    "/submit": {
                      "post": {
                        "operationId": "Submit",
                        "parameters": [{ "name": "id", "in": "query", "required": true, "schema": { "type": "integer" } }],
                        "requestBody": {
                          "x-name": "payload",
                          "required": true,
                          "content": { "application/json": { "schema": { "type": "string" } } }
                        },
                        "responses": { "200": { "description": "OK" } }
                      }
                    }
                  }
                }
                """);
            var before = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings()).GenerateFile();
            Assert.Contains("string payload", before);

            document.Paths["/submit"][OpenApiOperationMethod.Post].RequestBody = null;
            var after = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings()).GenerateFile();

            Assert.DoesNotContain("string payload", after);
            Assert.DoesNotContain("SerializeObject(payload", after);
            Assert.Contains("int id", after);
            CSharpCompiler.AssertCompile(before);
            CSharpCompiler.AssertCompile(after);
        }
    }
}
