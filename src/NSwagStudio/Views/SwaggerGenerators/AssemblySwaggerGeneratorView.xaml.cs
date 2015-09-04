using System.Threading.Tasks;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class AssemblySwaggerGeneratorView : ISwaggerGenerator
    {
        public AssemblySwaggerGeneratorView()
        {
            InitializeComponent();
        }

        private AssemblySwaggerGeneratorViewModel Model { get { return (AssemblySwaggerGeneratorViewModel)Resources["ViewModel"]; } }

        public string Title { get { return "Assembly"; } }

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
