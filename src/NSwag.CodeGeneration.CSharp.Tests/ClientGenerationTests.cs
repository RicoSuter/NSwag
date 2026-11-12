using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests;

public class ClientGenerationTests
{
    [Fact]
    public async Task CanGenerateFromJiraOpenApiSpecification()
    {
        await VerifyOutput("JIRA_OpenAPI", "jira-open-api.json", compile: false);
    }

    [Fact]
    public async Task CanGenerateFromShipBobOpenApiSpecification()
    {
        await VerifyOutput("ShipBob_OpenAPI", "shipbob-2025-07.json");
    }

    [Fact]
    public async Task CanGenerateFromNhsSpineServicesOpenApiSpecification()
    {
        await VerifyOutput("NHS_SpineServices_OpenAPI", "nhs-spineservices.json");
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

        var settings = new CSharpClientGeneratorSettings();
        settings.CSharpGeneratorSettings.GenerateEnumAsString = true;
        var code = new CSharpClientGenerator(document, settings).GenerateFile();

        Assert.Contains("public string Status", code);
        Assert.Contains("Allowed values: \"open\", \"closed\".", code);
        Assert.DoesNotContain("enum Status", code);
        CSharpCompiler.AssertCompile(code);
    }

    private static async Task VerifyOutput(string name, string fileName, bool compile = true)
    {
        var specification = await File.ReadAllTextAsync(Path.Combine("TestData", fileName));

        var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
        var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
        {
            GenerateOptionalParameters = true,
        });

        var code = generator.GenerateFile();

        await VerifyHelper
            .Verify(code, scrubApiComments: false)
            .UseFileName(name);

        if (compile)
        {
            CSharpCompiler.AssertCompile(code);
        }
    }
}