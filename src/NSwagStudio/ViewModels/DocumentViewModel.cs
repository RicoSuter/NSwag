using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Utilities;
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
            GenerateCommand = new AsyncRelayCommand(GenerateAsync);
        }

        /// <summary>Gets or sets the command to generate code from the selected Swagger generator.</summary>
        public AsyncRelayCommand GenerateCommand { get; set; }

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

        private async Task GenerateAsync()
        {
            var swaggerCode = await SwaggerGenerators[Document.SelectedSwaggerGenerator].GenerateSwaggerAsync();
            foreach (var generator in CodeGenerators)
                await generator.GenerateClientAsync(swaggerCode);
        }

        private void LoadGeneratoers(NSwagDocument document)
        {
            SwaggerGenerators = new ISwaggerGenerator[]
            {
                new SwaggerInputView(Document),
                new WebApiToSwaggerGeneratorView(Document.WebApiToSwaggerCommand),
                new JsonSchemaInputView(Document),
                new AssemblyTypeToSwaggerGeneratorView(Document.AssemblyTypeToSwaggerCommand),
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