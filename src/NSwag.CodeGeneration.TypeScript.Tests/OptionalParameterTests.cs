using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class OptionalParameterTests
    {
        [Fact]
        public async Task When_optional_parameter_comes_before_required()
        {
            // Arrange
            const string specification = """
                                         {
                                           "openapi": "3.0.0",
                                           "paths": {
                                             "/": {
                                               "get": {
                                                 "operationId": "Get",
                                                 "parameters": [
                                                   {
                                                     "name": "firstname",
                                                     "in": "query",
                                                     "schema": {
                                                       "type": "string",
                                                       "nullable": true
                                                     },
                                                     "x-position": 1
                                                   },
                                                   {
                                                     "name": "lastname",
                                                     "in": "query",
                                                     "required": true,
                                                     "schema": {
                                                       "type": "string"
                                                     },
                                                     "x-position": 2
                                                   }
                                                 ],
                                                 "responses": {
                                                   "200": {
                                                     "description": "",
                                                     "content": {
                                                       "application/json": {
                                                         "schema": {
                                                           "type": "string"
                                                         }
                                                       }
                                                     }
                                                   }
                                                 }
                                               }
                                             }
                                           }
                                         }
                                         """;

            // Act
            var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                GenerateOptionalParameters = true
            });
            var code = generator.GenerateFile();

            // Assert
            await VerifyHelper.Verify(code);
            CodeCompiler.AssertCompile(code);
        }
    }
}