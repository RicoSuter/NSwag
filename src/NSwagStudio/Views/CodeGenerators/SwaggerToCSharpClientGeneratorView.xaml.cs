using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

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
            Model.Command = document.CodeGenerators.SwaggerToCSharpClientCommand;
        }

        public override string Title => "CSharp Client";

        private SwaggerToCSharpClientGeneratorViewModel Model => (SwaggerToCSharpClientGeneratorViewModel)Resources["ViewModel"];

        public override async Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            await Model.GenerateClientAsync(document, documentPath);
            TabControl.SelectedIndex = 1;
        }

        public override bool IsActive
        {
            get { return _document.CodeGenerators.SwaggerToCSharpClientCommand != null; }
            set
            {
                if (value != IsActive)
                {
                    _document.CodeGenerators.SwaggerToCSharpClientCommand = value ? new SwaggerToCSharpClientCommand() : null;
                    Model.Command = _document.CodeGenerators.SwaggerToCSharpClientCommand;
                    OnPropertyChanged();
                }
            }
        }
    }
}
