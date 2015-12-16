using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Mvvm;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands;
using NSwagStudio.ViewModels;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblySwaggerGeneratorView : ISwaggerGenerator
    {
        public AssemblySwaggerGeneratorView(AssemblyTypeToSwaggerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        private AssemblySwaggerGeneratorViewModel Model { get { return (AssemblySwaggerGeneratorViewModel)Resources["ViewModel"]; } }

        public string Title { get { return ".NET Assembly"; } }
        
        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
