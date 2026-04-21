using Avalonia.Controls;
using NSwag.Commands;
using NSwag.Commands.Generation.AspNetCore;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators;

public partial class AspNetCoreToSwaggerGeneratorView : UserControl, ISwaggerGeneratorView
{
    public AspNetCoreToSwaggerGeneratorView(AspNetCoreToOpenApiCommand command, NSwagDocument document)
    {
        InitializeComponent();
        Model.Command = command;
        Model.Document = document;
    }

    private AspNetCoreToSwaggerGeneratorViewModel Model => (AspNetCoreToSwaggerGeneratorViewModel)Resources["ViewModel"]!;

    public string Title => "ASP.NET Core";

    public IOutputCommand Command => Model.Command;

    public Task<string> GenerateSwaggerAsync()
    {
        return Model.GenerateSwaggerAsync();
    }

    public override string ToString()
    {
        return Title;
    }
}
