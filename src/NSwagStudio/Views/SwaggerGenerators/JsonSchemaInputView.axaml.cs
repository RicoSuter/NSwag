using Avalonia.Controls;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.Helpers;

namespace NSwagStudio.Views.SwaggerGenerators;

public partial class JsonSchemaInputView : UserControl, ISwaggerGeneratorView
{
    private readonly JsonSchemaToOpenApiCommand _command;

    public JsonSchemaInputView(JsonSchemaToOpenApiCommand command)
    {
        _command = command;
        InitializeComponent();
        DataContext = command;
    }

    public string Title => "JSON Schema";

    public IOutputCommand Command => _command;

    public async Task<string> GenerateSwaggerAsync()
    {
        try
        {
            return await Task.Run(async () =>
            {
                var document = await _command.RunAsync().ConfigureAwait(false);
                return document.ToJson();
            });
        }
        catch (Exception exception)
        {
            await MessageBoxHelper.ShowError("Error loading the JSON Schema", "Error", exception);
            return string.Empty;
        }
    }
}
