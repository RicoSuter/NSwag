using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.ViewModels.CodeGenerators;
using System.Linq;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToTypeScriptClientGeneratorView
    {
        private readonly NSwagDocument _document;

        public SwaggerToTypeScriptClientGeneratorView(NSwagDocument document)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            _document = document;
            Model.Command = document.CodeGenerators.OpenApiToTypeScriptClientCommands?.FirstOrDefault();
        }

        private SwaggerToTypeScriptClientGeneratorViewModel Model => (SwaggerToTypeScriptClientGeneratorViewModel)Resources["ViewModel"];

        public override string Title => "TypeScript Client";

        public override void UpdateOutput(OpenApiDocumentExecutionResult result)
        {
            Model.ClientCode = result.GetGeneratorOutput<OpenApiToTypeScriptClientCommand>();
            if (result.IsRedirectedOutput)
                TabControl.SelectedIndex = 1;
        }

        public override bool IsSelected
        {
            get { return _document.CodeGenerators.OpenApiToTypeScriptClientCommands?.FirstOrDefault() != null; }
            set
            {
                if (value != IsSelected)
                {
                    _document.CodeGenerators.OpenApiToTypeScriptClientCommands = value ? new OpenApiToTypeScriptClientCommand[] { new OpenApiToTypeScriptClientCommand() } : null;
                    Model.Command = _document.CodeGenerators.OpenApiToTypeScriptClientCommands?.FirstOrDefault();
                    OnPropertyChanged();
                }
            }
        }
    }
}
