using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToCSharpControllerGeneratorView
    {
        private readonly NSwagDocument _document;

        public SwaggerToCSharpControllerGeneratorView(NSwagDocument document)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);

            _document = document;
            Model.Command = document.CodeGenerators.OpenApiToCSharpControllerCommand;
        }

        public override string Title => "CSharp Controller";

        private SwaggerToCSharpControllerGeneratorViewModel Model => (SwaggerToCSharpControllerGeneratorViewModel)Resources["ViewModel"];

        public override void UpdateOutput(OpenApiDocumentExecutionResult result)
        {
            Model.ClientCode = result.GetGeneratorOutput<OpenApiToCSharpControllerCommand>();
            if (result.IsRedirectedOutput)
                TabControl.SelectedIndex = 1;
        }

        public override bool IsSelected
        {
            get { return _document.CodeGenerators.OpenApiToCSharpControllerCommand != null; }
            set
            {
                if (value != IsSelected)
                {
                    _document.CodeGenerators.OpenApiToCSharpControllerCommand = value ? new OpenApiToCSharpControllerCommand() : null;
                    Model.Command = _document.CodeGenerators.OpenApiToCSharpControllerCommand;
                    OnPropertyChanged();
                }
            }
        }
    }
}
