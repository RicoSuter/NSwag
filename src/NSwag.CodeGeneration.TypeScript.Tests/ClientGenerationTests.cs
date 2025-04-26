using NJsonSchema;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests;

public class ClientGenerationTests
{
    [Fact]
    public async Task CanGenerateFromJiraOpenApiSpecification()
    {
        await VerifyOutput("JIRA_OpenAPI", "jira-open-api.json");
    }

    private static async Task VerifyOutput(string name, string fileName)
    {
        var specification = await File.ReadAllTextAsync(Path.Combine("TestData", fileName));

        var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
        var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
        {
            GenerateOptionalParameters = true,
        });

        var code = generator.GenerateFile();

        await VerifyHelper.Verify(code, scrubApiComments: false)
            .UseFileName(name)
            .UseDirectory("Snapshots");;
    }
}