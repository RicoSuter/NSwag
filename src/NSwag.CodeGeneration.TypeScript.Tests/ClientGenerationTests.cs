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