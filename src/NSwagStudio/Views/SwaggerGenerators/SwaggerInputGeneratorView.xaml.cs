using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputGeneratorView : ISwaggerGenerator
    {
        public SwaggerInputGeneratorView(NSwagDocument document)
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

        private SwaggerInputGeneratorViewModel Model => (SwaggerInputGeneratorViewModel)Resources["ViewModel"];

        public NSwagDocument Document { get; set; }

        public string Title => "Swagger Specification";

        public async Task<string> GenerateSwaggerAsync()
        {
            return Model.Document.InputSwagger;
        }
    }
}
