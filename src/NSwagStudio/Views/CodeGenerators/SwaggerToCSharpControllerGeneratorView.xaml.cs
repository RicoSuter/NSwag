using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag;
using NSwag.Commands;
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
            Model.Command = document.CodeGenerators.SwaggerToCSharpControllerCommand;
        }

        public override string Title => "CSharp Web API Controller (experimental)";

        private SwaggerToCSharpControllerGeneratorViewModel Model => (SwaggerToCSharpControllerGeneratorViewModel)Resources["ViewModel"];

        public override async Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            await Model.GenerateClientAsync(document, documentPath);
            TabControl.SelectedIndex = 1;
        }

        public override bool IsSelected
        {
            get { return _document.CodeGenerators.SwaggerToCSharpControllerCommand != null; }
            set
            {
                if (value != IsSelected)
                {
                    _document.CodeGenerators.SwaggerToCSharpControllerCommand = value ? new SwaggerToCSharpControllerCommand() : null;
                    Model.Command = _document.CodeGenerators.SwaggerToCSharpControllerCommand;
                    OnPropertyChanged();
                }
            }
        }
    }
}
