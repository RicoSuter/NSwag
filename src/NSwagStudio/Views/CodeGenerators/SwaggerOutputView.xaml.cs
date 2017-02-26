using System.Threading.Tasks;
using NSwag;
using NSwagStudio.ViewModels.CodeGenerators;

namespace NSwagStudio.Views.CodeGenerators
{
    public partial class SwaggerOutputView
    {
        public SwaggerOutputView()
        {
            InitializeComponent();
        }

        public override string Title => "Swagger Specification";

        private SwaggerOutputViewModel Model => (SwaggerOutputViewModel)Resources["ViewModel"];

        public override Task GenerateClientAsync(SwaggerDocument document, string documentPath)
        {
            return Model.GenerateClientAsync(document, documentPath);
        }

        public override bool IsSelected
        {
            get { return true; }
            set { }
        }

        public override bool IsPersistent => true;
    }
}
