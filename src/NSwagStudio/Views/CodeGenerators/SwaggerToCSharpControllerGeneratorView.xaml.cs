using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToCSharpControllerGeneratorView : ICodeGeneratorView
    {
        public SwaggerToCSharpControllerGeneratorView(SwaggerToCSharpControllerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        public string Title => "CSharp Web API Controller (experimental)";

        private SwaggerToCSharpControllerGeneratorViewModel Model => (SwaggerToCSharpControllerGeneratorViewModel) Resources["ViewModel"];

        public async Task GenerateClientAsync(string swaggerData, string documentPath)
        {
            await Model.GenerateClientAsync(swaggerData, documentPath);
            TabControl.SelectedIndex = 1;
        }
    }
}
