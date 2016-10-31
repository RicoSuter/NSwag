using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Utilities;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.Commands;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    public class DocumentViewModel : ViewModelBase
    {
        private static NSwagDocument _document;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public DocumentViewModel()
        {
            GenerateCommand = new AsyncRelayCommand<string>(GenerateAsync);
        }

        /// <summary>Gets or sets the command to generate code from the selected Swagger generator.</summary>
        public AsyncRelayCommand<string> GenerateCommand { get; set; }

        /// <summary>Gets the swagger generators.</summary>
        public ISwaggerGenerator[] SwaggerGenerators { get; private set; }

        /// <summary>Gets the client generators.</summary>
        public ICodeGenerator[] CodeGenerators { get; private set; }

        /// <summary>Gets or sets the settings. </summary>
        public NSwagDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;
                    if (value != null)
                        LoadGeneratoers(value);
                    RaisePropertyChanged(() => Document);
                }
            }
        }

        /// <summary>Gets the application version with build time. </summary>
        public string ApplicationVersion => GetType().Assembly.GetVersionWithBuildTime();

        private async Task GenerateAsync(string type)
        {
            IsLoading = true;

            if (type == "files")
            {
                try
                {
                    await Document.ExecuteAsync();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Error in DocumentViewModel.GenerateAsync: " + exception);
                }
            }
            else
            {
                var generator = SwaggerGenerators[Document.SelectedSwaggerGenerator];

                var documentPath = generator is SwaggerInputView && !string.IsNullOrEmpty(Document.InputSwaggerUrl) ?
                    Document.InputSwaggerUrl : null;

                var swaggerCode = await generator.GenerateSwaggerAsync();
                foreach (var codeGenerator in CodeGenerators)
                    await codeGenerator.GenerateClientAsync(swaggerCode, documentPath);
            }

            IsLoading = false;
        }

        private void LoadGeneratoers(NSwagDocument document)
        {
            SwaggerGenerators = new ISwaggerGenerator[]
            {
                new SwaggerInputView(Document),
                new WebApiToSwaggerGeneratorView((WebApiToSwaggerCommand) Document.WebApiToSwaggerCommand),
                new JsonSchemaInputView(Document),
                new AssemblyTypeToSwaggerGeneratorView((AssemblyTypeToSwaggerCommand) Document.AssemblyTypeToSwaggerCommand),
            };

            CodeGenerators = new ICodeGenerator[]
            {
                new SwaggerOutputView(),
                new SwaggerToTypeScriptClientGeneratorView(Document.SwaggerToTypeScriptClientCommand),
                new SwaggerToCSharpClientGeneratorView(Document.SwaggerToCSharpClientCommand),
                new SwaggerToCSharpControllerGeneratorView(Document.SwaggerToCSharpControllerCommand)
            };

            RaisePropertyChanged(() => SwaggerGenerators);
            RaisePropertyChanged(() => CodeGenerators);
        }
    }
}