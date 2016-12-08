using System;
using System.Threading.Tasks;
using System.Windows;
using NSwag.Commands;
using NSwag.Commands.Base;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class JsonSchemaInputView : ISwaggerGeneratorView
    {
        private readonly JsonSchemaToSwaggerCommand _command;

        public JsonSchemaInputView(JsonSchemaToSwaggerCommand command)
        {
            _command = command;
            InitializeComponent();
            DataContext = command;
        }

        public string Title => "JSON Schema";

        public OutputCommandBase Command => _command;

        public async Task<string> GenerateSwaggerAsync()
        {
            try
            {
                var document = await _command.RunAsync();
                return await Task.Run(() => document.ToJson());
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error loading the JSON Schema");
                return string.Empty; // TODO: What to do on error?
            }
        }
    }
}
