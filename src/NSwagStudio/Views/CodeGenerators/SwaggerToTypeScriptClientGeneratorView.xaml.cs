using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToTypeScriptClientGeneratorView : ICodeGeneratorView
    {
        public SwaggerToTypeScriptClientGeneratorView(SwaggerToTypeScriptClientCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        private SwaggerToTypeScriptClientGeneratorViewModel Model => (SwaggerToTypeScriptClientGeneratorViewModel)Resources["ViewModel"];

        public string Title => "TypeScript Client";

        public async Task GenerateClientAsync(string swaggerData, string documentPath)
        {
            await Model.GenerateClientAsync(swaggerData, documentPath);
            TabControl.SelectedIndex = 1;
        }

        public override string ToString()
        {
            return Title; 
        }
    }
}
