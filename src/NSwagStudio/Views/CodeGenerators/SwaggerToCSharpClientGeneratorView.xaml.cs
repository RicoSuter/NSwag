using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToCSharpClientGeneratorView : ICodeGeneratorView
    {
        public SwaggerToCSharpClientGeneratorView(SwaggerToCSharpClientCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        public string Title => "CSharp Client";

        private SwaggerToCSharpClientGeneratorViewModel Model => (SwaggerToCSharpClientGeneratorViewModel) Resources["ViewModel"];

        public async Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            await Model.GenerateClientAsync(document, documentPath);
            TabControl.SelectedIndex = 1;
        }
    }
}
