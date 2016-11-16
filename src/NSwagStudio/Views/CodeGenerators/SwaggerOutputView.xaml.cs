using System.Threading.Tasks;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerOutputView : ICodeGeneratorView
    {
        public SwaggerOutputView()
        {
            InitializeComponent();
        }

        public string Title => "Swagger Specification";

        private SwaggerOutputViewModel Model => (SwaggerOutputViewModel)Resources["ViewModel"];

        public Task GenerateClientAsync(string swaggerData, string documentPath)
        {
            return Model.GenerateClientAsync(swaggerData, documentPath);
        }
    }
}
