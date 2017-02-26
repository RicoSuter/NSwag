using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

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
            Model.Command = document.CodeGenerators.SwaggerToTypeScriptClientCommand;
        }

        private SwaggerToTypeScriptClientGeneratorViewModel Model => (SwaggerToTypeScriptClientGeneratorViewModel)Resources["ViewModel"];

        public override string Title => "TypeScript Client";

        public override async Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            await Model.GenerateClientAsync(document, documentPath);
            TabControl.SelectedIndex = 1;
        }

        public override bool IsSelected
        {
            get { return _document.CodeGenerators.SwaggerToTypeScriptClientCommand != null; }
            set
            {
                if (value != IsSelected)
                {
                    _document.CodeGenerators.SwaggerToTypeScriptClientCommand = value ? new SwaggerToTypeScriptClientCommand() : null;
                    Model.Command = _document.CodeGenerators.SwaggerToTypeScriptClientCommand;
                    OnPropertyChanged();
                }
            }
        }
    }
}
