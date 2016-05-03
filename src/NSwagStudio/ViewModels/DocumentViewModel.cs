using System.Threading.Tasks;
using MyToolkit.Command;
using MyToolkit.Utilities;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;
using CSharpClientGeneratorView = NSwagStudio.Views.CodeGenerators.CSharpClientGeneratorView;
using TypeScriptClientGeneratorView = NSwagStudio.Views.CodeGenerators.TypeScriptClientGeneratorView;

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
                new SwaggerInputGeneratorView(Document),
                new WebApiSwaggerGeneratorView(Document.WebApiToSwaggerCommand),
                new JsonSchemaInputGeneratorView(Document),
                new AssemblySwaggerGeneratorView(Document.AssemblyTypeToSwaggerCommand),
            };

            CodeGenerators = new ICodeGenerator[]
            {
                new SwaggerGeneratorView(),
                new TypeScriptClientGeneratorView(Document.SwaggerToTypeScriptCommand),
                new CSharpClientGeneratorView(Document.SwaggerToCSharpClientCommand),
                new CSharpWebApiControllerGeneratorView(Document.SwaggerToCSharpWebApiControllerCommand)
            };

            RaisePropertyChanged(() => SwaggerGenerators);
            RaisePropertyChanged(() => CodeGenerators);
        }               
    }
}