using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NSwag.Commands;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels;

public class DocumentModel : ObservableObject
{
    public NSwagDocument Document { get; }

    /// <summary>Gets the swagger generators.</summary>
    public ISwaggerGeneratorView[] SwaggerGeneratorViews { get; }

    public IReadOnlyCollection<CodeGeneratorModel> CodeGenerators { get; }

    public IEnumerable<CodeGeneratorModel> SelectedCodeGenerators => CodeGenerators.Where(c => c.View.IsSelected || c.View.IsPersistent);

    public DocumentModel(NSwagDocument document)
    {
        Document = document;

        SwaggerGeneratorViews = new ISwaggerGeneratorView[]
        {
            new SwaggerInputView(Document.SwaggerGenerators.FromDocumentCommand),
            new AspNetCoreToSwaggerGeneratorView(Document.SwaggerGenerators.AspNetCoreToOpenApiCommand, document),
            new JsonSchemaInputView(Document.SwaggerGenerators.JsonSchemaToOpenApiCommand),
        };

        CodeGenerators = new CodeGeneratorViewBase[]
        {
            new SwaggerOutputView(),
            new SwaggerToTypeScriptClientGeneratorView(Document),
            new SwaggerToCSharpClientGeneratorView(Document),
            new SwaggerToCSharpControllerGeneratorView(Document)
        }
        .Select(v => new CodeGeneratorModel { View = v })
        .ToList();

        foreach (var codeGenerator in CodeGenerators)
            codeGenerator.View.PropertyChanged += OnCodeGeneratorPropertyChanged;

        OnPropertyChanged(nameof(SwaggerGeneratorViews));
        OnPropertyChanged(nameof(CodeGenerators));
    }

    private void OnCodeGeneratorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CodeGeneratorViewBase.IsSelected))
            OnPropertyChanged(nameof(SelectedCodeGenerators));
    }

    public ISwaggerGeneratorView GetSwaggerGeneratorView()
    {
        return SwaggerGeneratorViews.Single(g => g.Command == Document.SelectedSwaggerGenerator);
    }

    public string? GetDocumentPath(ISwaggerGeneratorView generator)
    {
        return generator is SwaggerInputView && !string.IsNullOrEmpty(Document.SwaggerGenerators.FromDocumentCommand.Url)
            ? Document.SwaggerGenerators.FromDocumentCommand.Url
            : null;
    }
}
