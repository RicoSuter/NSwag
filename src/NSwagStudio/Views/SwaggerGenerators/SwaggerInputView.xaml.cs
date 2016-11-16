using System.Threading.Tasks;
using NSwag.CodeGeneration;
using NSwag.Commands;
using NSwag.Commands.Base;
using NSwagStudio.ViewModels.SwaggerGenerators;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class SwaggerInputView : ISwaggerGeneratorView
    {
        private readonly FromSwaggerCommand _command;

        public SwaggerInputView(FromSwaggerCommand command)
        {
            _command = command;
            InitializeComponent();

            var hasInputSwaggerUrl = !string.IsNullOrEmpty(_command.Url);
            if (hasInputSwaggerUrl)
                _command.Swagger = string.Empty;

            Model.Command = command;
            Model.RaiseAllPropertiesChanged();

            if (hasInputSwaggerUrl)
                Model.LoadSwaggerUrlAsync();
        }

        public OutputCommandBase Command => _command;

        private SwaggerInputViewModel Model => (SwaggerInputViewModel)Resources["ViewModel"];

        public NSwagDocument Document { get; set; }

        public string Title => "Swagger Specification";

        public async Task<string> GenerateSwaggerAsync()
        {
            return Model.Command.Swagger;
        }
    }
}
