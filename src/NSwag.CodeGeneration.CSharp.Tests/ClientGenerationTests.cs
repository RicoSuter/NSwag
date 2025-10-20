using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.CSharp.Tests;

public class ClientGenerationTests
{
    [Fact]
    public async Task CanGenerateFromJiraOpenApiSpecification()
    {
        // Jira's OpenAPI spec generates code like this:
        //// public bool ShowDaysInColumn { get; set; } = MyNamespace.bool.False;
        await VerifyOutput("JIRA_OpenAPI", "jira-open-api.json", compile: false);
    }

    [Fact]
    public async Task CanGenerateFromShipBobOpenApiSpecification()
    {
        await VerifyOutput("ShipBob_OpenAPI", "shipbob-2025-07.json", compile: true);
    }

    [Fact]
    public async Task CanGenerateFromNhsSpineServicesOpenApiSpecification()
    {
        await VerifyOutput("NHS_SpineServices_OpenAPI", "nhs-spineservices.json", compile: true);
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