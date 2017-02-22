using System.Threading.Tasks;
using NSwag;
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

        public Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            return Model.GenerateClientAsync(document, documentPath);
        }

        public string IsSelected { get; set; }
    }
}
