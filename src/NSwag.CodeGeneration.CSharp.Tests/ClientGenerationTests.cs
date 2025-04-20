using NJsonSchema;

namespace NSwag.CodeGeneration.CSharp.Tests;

public class ClientGenerationTests
{
    [Fact]
    public async Task CanGenerateFromJiraOpenApiSpecification()
    {
        await VerifyOutput("JIRA_OpenAPI", "jira-open-api.json");
    }

    private static async Task VerifyOutput(string name, string fileName)
    {
        Environment.SetEnvironmentVariable("NSWAG_NOVERSION", "true");

        var specification = await File.ReadAllTextAsync(Path.Combine("TestData", fileName));

        var document = await OpenApiDocument.FromJsonAsync(specification, "", SchemaType.OpenApi3);
        var generator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings
        {
            GenerateOptionalParameters = true,
        });

        var code = generator.GenerateFile();

        await Verify(code)
            .UseFileName(name)
            .ScrubLinesContaining(StringComparison.OrdinalIgnoreCase, "Generated using the NSwag toolchain")
            .UseDirectory("Snapshots");
        ;
    }
}