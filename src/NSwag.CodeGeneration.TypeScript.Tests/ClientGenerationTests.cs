using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests;

public class ClientGenerationTests
{
    [Theory]
    [InlineData(TypeScriptTemplate.Angular)]
    [InlineData(TypeScriptTemplate.AngularJS)]
    [InlineData(TypeScriptTemplate.Aurelia)]
    [InlineData(TypeScriptTemplate.Axios)]
    [InlineData(TypeScriptTemplate.Fetch)]
    [InlineData(TypeScriptTemplate.JQueryCallbacks)]
    [InlineData(TypeScriptTemplate.JQueryPromises)]
    public async Task CanGenerateFromJiraOpenApiSpecification(TypeScriptTemplate template)
    {
        await VerifyOutput("JIRA_OpenAPI", "jira-open-api.json", template);
    }

    [Fact]
    public async Task GeneratesOpenStringEnumsWhenEnabled()
    {
        var document = await OpenApiDocument.FromJsonAsync(
            """
            {
              "openapi": "3.0.0",
              "info": { "title": "Enums", "version": "1.0" },
              "paths": {
                "/status": {
                  "get": {
                    "operationId": "GetStatus",
                    "responses": {
                      "200": {
                        "description": "Success",
                        "content": {
                          "application/json": {
                            "schema": { "$ref": "#/components/schemas/StatusDto" }
                          }
                        }
                      }
                    }
                  }
                }
              },
              "components": {
                "schemas": {
                  "Status": { "type": "string", "enum": ["open", "closed"] },
                  "StatusDto": {
                    "type": "object",
                    "properties": {
                      "status": { "$ref": "#/components/schemas/Status" }
                    }
                  }
                }
              }
            }
            """,
            "",
            SchemaType.OpenApi3);

        var settings = new TypeScriptClientGeneratorSettings();
        settings.TypeScriptGeneratorSettings.GenerateEnumAsString = true;
        var code = new TypeScriptClientGenerator(document, settings).GenerateFile();

        Assert.Matches(@"status(?:\?|!)?\s*:\s*string(?:\s*\|\s*undefined)?", code);
        Assert.Contains("Allowed values: \"open\", \"closed\".", code);
        Assert.DoesNotContain("enum Status", code);
    }

    private static async Task VerifyOutput(string name, string fileName, TypeScriptTemplate template)
    {
        var specification = await File.ReadAllTextAsync(Path.Combine("TestData", fileName));

        var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
        var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
        {
            GenerateOptionalParameters = true,
            Template = template
        });

        var code = generator.GenerateFile();

        await VerifyHelper.Verify(code, scrubApiComments: false)
            .UseFileName($"{name}_{template}")
            .UseDirectory("Snapshots");
    }
}