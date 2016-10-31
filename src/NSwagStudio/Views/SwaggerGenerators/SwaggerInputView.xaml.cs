using System.Threading.Tasks;
using NSwag.CodeGeneration;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputView : ISwaggerGenerator
    {
        public SwaggerInputView(NSwagDocument document)
        {
            InitializeComponent();

            var hasInputSwaggerUrl = !string.IsNullOrEmpty(document.InputSwaggerUrl);
            if (hasInputSwaggerUrl)
                document.InputSwagger = string.Empty;

            Model.Document = document;
            Model.RaiseAllPropertiesChanged();

            if (hasInputSwaggerUrl)
                Model.LoadSwaggerUrlAsync();
        }

        private SwaggerInputViewModel Model => (SwaggerInputViewModel)Resources["ViewModel"];

        public NSwagDocument Document { get; set; }

        public string Title => "Swagger Specification";

        public async Task<string> GenerateSwaggerAsync()
        {
            return Model.Document.InputSwagger;
        }
    }
}
