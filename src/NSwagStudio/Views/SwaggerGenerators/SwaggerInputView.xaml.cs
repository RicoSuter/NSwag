using System.Threading.Tasks;
using NSwag.Commands;
using NSwag.Commands.Generation;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputView : ISwaggerGeneratorView
    {
        private readonly FromDocumentCommand _command;

        public SwaggerInputView(FromDocumentCommand command)
        {
            _command = command;
            InitializeComponent();

            Model.Command = command;
            Model.RaiseAllPropertiesChanged();
        }

        public IOutputCommand Command => _command;

        private SwaggerInputViewModel Model => (SwaggerInputViewModel)Resources["ViewModel"];

        public NSwagDocument Document { get; set; }

        public string Title => "OpenAPI/Swagger Specification";

        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }
    }
}
