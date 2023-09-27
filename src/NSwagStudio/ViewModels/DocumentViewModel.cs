using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MyToolkit.Command;
using MyToolkit.Utilities;
using NSwag.Commands;

namespace NSwagStudio.ViewModels
{
    public class DocumentViewModel : ViewModelBase
    {
        private DocumentModel _document;

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

        /// <summary>Gets the available runtimes.</summary>
        public Runtime[] Runtimes
        {
            get
            {
                return Enum.GetNames(typeof(Runtime))
                    .Select(t => (Runtime)Enum.Parse(typeof(Runtime), t))
                    .ToArray();
            }
        }

        private async Task GenerateAsync(string type)
        {
            IsLoading = true;
            await RunTaskAsync(async () =>
            {
                var redirectOutput = type != "files";

                var start = Stopwatch.GetTimestamp();
                var result = await Document.Document.ExecuteCommandLineAsync(redirectOutput);
                var duration = TimeSpan.FromSeconds((Stopwatch.GetTimestamp() - start) / Stopwatch.Frequency);

                if (redirectOutput)
                {
                    foreach (var codeGenerator in Document.CodeGenerators)
                        codeGenerator.View.UpdateOutput(result);
                }
                else
                {
                    foreach (var codeGenerator in Document.CodeGenerators)
                        codeGenerator.View.UpdateOutput(result);

#pragma warning disable CS4014
                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MessageBox.Show("File: " + Document.Document.Path + "\nDuration: " + duration, "Generation complete!");
                    });
#pragma warning restore CS4014
                }
            });
            IsLoading = false;
        }
    }
}