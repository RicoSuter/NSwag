using Avalonia.Controls;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators;

public partial class SwaggerToCSharpClientGeneratorView : CodeGeneratorViewBase
{
    private readonly NSwagDocument _document;

    public SwaggerToCSharpClientGeneratorView(NSwagDocument document)
    {
        InitializeComponent();
        _document = document;
        Model.Command = document.CodeGenerators.OpenApiToCSharpClientCommand;
    }

    public override string Title => "CSharp Client";

    private SwaggerToCSharpClientGeneratorViewModel Model => (SwaggerToCSharpClientGeneratorViewModel)Resources["ViewModel"]!;

    public override void UpdateOutput(OpenApiDocumentExecutionResult result)
    {
        Model.ClientCode = result.GetGeneratorOutput<OpenApiToCSharpClientCommand>();
        if (result.IsRedirectedOutput)
        {
            var tabControl = this.FindControl<Avalonia.Controls.TabControl>("TabControl");
            if (tabControl != null)
                tabControl.SelectedIndex = 1;
        }
    }

    public override bool IsSelected
    {
        get => _document.CodeGenerators.OpenApiToCSharpClientCommand != null;
        set
        {
            if (value != IsSelected)
            {
                _document.CodeGenerators.OpenApiToCSharpClientCommand = value ? new OpenApiToCSharpClientCommand() : null;
                Model.Command = _document.CodeGenerators.OpenApiToCSharpClientCommand!;
                OnPropertyChanged();
            }
        }
    }
}
