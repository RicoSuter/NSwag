﻿using System.Windows;
using NSwag.Commands;
using NSwag.Commands.CodeGeneration;

namespace NSwagStudio.Views.SwaggerGenerators
{
    public partial class JsonSchemaInputView : ISwaggerGeneratorView
    {
        private readonly JsonSchemaToOpenApiCommand _command;

        public JsonSchemaInputView(JsonSchemaToOpenApiCommand command)
        {
            _command = command;
            InitializeComponent();
            DataContext = command;
        }

        public string Title => "JSON Schema";

        public IOutputCommand Command => _command;

        public async Task<string> GenerateSwaggerAsync()
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var document = await _command.RunAsync().ConfigureAwait(false);
                    return document.ToJson();
                });
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error loading the JSON Schema");
                return string.Empty; // TODO: What to do on error?
            }
        }
    }
}
