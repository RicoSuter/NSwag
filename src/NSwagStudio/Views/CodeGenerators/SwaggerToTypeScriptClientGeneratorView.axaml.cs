using Avalonia.Controls;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators;

public partial class SwaggerToTypeScriptClientGeneratorView : CodeGeneratorViewBase
{
    private readonly NSwagDocument _document;

    public SwaggerToTypeScriptClientGeneratorView(NSwagDocument document)
    {
        InitializeComponent();
        _document = document;
        Model.Command = document.CodeGenerators.OpenApiToTypeScriptClientCommand;
    }

    private SwaggerToTypeScriptClientGeneratorViewModel Model => (SwaggerToTypeScriptClientGeneratorViewModel)Resources["ViewModel"]!;

    public override string Title => "TypeScript Client";

    public override void UpdateOutput(OpenApiDocumentExecutionResult result)
    {
        Model.ClientCode = result.GetGeneratorOutput<OpenApiToTypeScriptClientCommand>();
        if (result.IsRedirectedOutput)
        {
            var tabControl = this.FindControl<Avalonia.Controls.TabControl>("TabControl");
            if (tabControl != null)
                tabControl.SelectedIndex = 1;
        }
    }

    public override bool IsSelected
    {
        get => _document.CodeGenerators.OpenApiToTypeScriptClientCommand != null;
        set
        {
            if (value != IsSelected)
            {
                _document.CodeGenerators.OpenApiToTypeScriptClientCommand = value ? new OpenApiToTypeScriptClientCommand() : null;
                Model.Command = _document.CodeGenerators.OpenApiToTypeScriptClientCommand!;
                OnPropertyChanged();
            }
        }
    }
}
