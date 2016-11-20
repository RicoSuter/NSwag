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

            Model.Command = command;
            Model.RaiseAllPropertiesChanged();
        }

        public OutputCommandBase Command => _command;

        private SwaggerInputViewModel Model => (SwaggerInputViewModel)Resources["ViewModel"];

        public NSwagDocument Document { get; set; }

        public string Title => "Swagger Specification";

        public Task<string> GenerateSwaggerAsync()
        {
            return Model.GenerateSwaggerAsync();
        }
    }
}
