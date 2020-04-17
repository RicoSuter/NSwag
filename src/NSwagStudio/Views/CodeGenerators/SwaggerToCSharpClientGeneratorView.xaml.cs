using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.ViewModels.CodeGenerators;
using System.Linq;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToCSharpClientGeneratorView
    {
        private readonly NSwagDocument _document;

        public SwaggerToCSharpClientGeneratorView(NSwagDocument document)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            _document = document;
            Model.Command = document.CodeGenerators.OpenApiToCSharpClientCommands.FirstOrDefault();
        }

        public override string Title => "CSharp Client";

        private SwaggerToCSharpClientGeneratorViewModel Model => (SwaggerToCSharpClientGeneratorViewModel)Resources["ViewModel"];

        public override void UpdateOutput(OpenApiDocumentExecutionResult result)
        {
            Model.ClientCode = result.GetGeneratorOutput<OpenApiToCSharpClientCommand>();
            if (result.IsRedirectedOutput)
                TabControl.SelectedIndex = 1;
        }

        public override bool IsSelected
        {
            get { return _document.CodeGenerators.OpenApiToCSharpClientCommands?.FirstOrDefault() != null; }
            set
            {
                if (value != IsSelected)
                {
                    _document.CodeGenerators.OpenApiToCSharpClientCommands = value ? new OpenApiToCSharpClientCommand[] { new OpenApiToCSharpClientCommand() } : null;
                    Model.Command = _document.CodeGenerators.OpenApiToCSharpClientCommands.FirstOrDefault();
                    OnPropertyChanged();
                }
            }
        }
    }
}
