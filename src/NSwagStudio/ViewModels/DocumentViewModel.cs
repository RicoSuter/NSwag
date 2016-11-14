using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Utilities;

namespace NSwagStudio.ViewModels
{
    public class DocumentViewModel : ViewModelBase
    {
        private static DocumentModel _document;

        /// <summary>Initializes a new instance of the <see cref="MainWindowModel"/> class.</summary>
        public DocumentViewModel()
        {
            GenerateCommand = new AsyncRelayCommand<string>(GenerateAsync);
        }

        /// <summary>Gets or sets the command to generate code from the selected Swagger generator.</summary>
        public AsyncRelayCommand<string> GenerateCommand { get; set; }

        public string SwaggerGenerator { get; set; }


        /// <summary>Gets or sets the settings. </summary>
        public DocumentModel Document
        {
            get { return _document; }
            set { Set(ref _document, value); }
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
                    await Document.Document.ExecuteAsync();
                }
                catch (Exception exception)
                {
                    Debug.WriteLine("Error in DocumentViewModel.GenerateAsync: " + exception);
                }
            }
            else
            {
                var generator = Document.GetSwaggerGeneratorView();
                var documentPath = Document.GetDocumentPath(generator);
                var swaggerCode = await generator.GenerateSwaggerAsync();
                foreach (var codeGenerator in Document.CodeGenerators)
                    await codeGenerator.GenerateClientAsync(swaggerCode, documentPath);
            }

            IsLoading = false;
        }
    }
}