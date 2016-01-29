using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class TypeScriptClientGeneratorView : ICodeGenerator
    {
        public TypeScriptClientGeneratorView(SwaggerToTypeScriptCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        private TypeScriptClientGeneratorViewModel Model { get { return (TypeScriptClientGeneratorViewModel)Resources["ViewModel"]; } }

        public string Title { get { return "TypeScript Client"; } }
        
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
