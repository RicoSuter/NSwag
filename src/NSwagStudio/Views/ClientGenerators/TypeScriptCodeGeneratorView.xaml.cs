using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;
using NSwagStudio.ViewModels;
using NSwagStudio.ViewModels.ClientGenerators;

namespace NSwagStudio.Views.ClientGenerators
{
    public partial class TypeScriptCodeGeneratorView : IClientGenerator
    {
        public TypeScriptCodeGeneratorView()
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
        }

        private TypeScriptCodeGeneratorViewModel Model { get { return (TypeScriptCodeGeneratorViewModel)Resources["ViewModel"]; } }

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
