using System.Threading.Tasks;
using MyToolkit.Mvvm;
using NSwag.Commands;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblyTypeToSwaggerGeneratorView : ISwaggerGenerator
    {
        public AssemblyTypeToSwaggerGeneratorView(AssemblyTypeToSwaggerCommand command)
        {
            InitializeComponent();
            ViewModelHelper.RegisterViewModel(Model, this);
            Model.Command = command; 
        }

        private AssemblyTypeToSwaggerGeneratorViewModel Model => (AssemblyTypeToSwaggerGeneratorViewModel)Resources["ViewModel"];

        public string Title => ".NET Assembly";

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
