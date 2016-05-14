using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerToTypeScriptClientGeneratorView : ICodeGenerator
    {
        public SwaggerToTypeScriptClientGeneratorView(SwaggerToTypeScriptClientCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        private SwaggerToTypeScriptClientGeneratorViewModel Model => (SwaggerToTypeScriptClientGeneratorViewModel)Resources["ViewModel"];

        public string Title => "TypeScript Client";

        public Task GenerateClientAsync(string swaggerData)
        {
            return Model.GenerateClientAsync(swaggerData);
        }

        public override string ToString()
        {
            return Title; 
        }
    }
}
