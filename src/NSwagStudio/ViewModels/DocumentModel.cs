using System.Linq;
using MyToolkit.Model;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.Commands;
using NSwag.Commands;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    public class DocumentModel : ObservableObject
    {
        public NSwagDocument Document { get; private set; }

        /// <summary>Gets the swagger generators.</summary>
        public ISwaggerGeneratorView[] SwaggerGenerators { get; private set; }

        /// <summary>Gets the client generators.</summary>
        public ICodeGeneratorView[] CodeGenerators { get; private set; }

        public DocumentModel(NSwagDocument document)
        {
            Document = document;
            LoadGenerators();
        }

        private void LoadGenerators()
        {
            SwaggerGenerators = new ISwaggerGeneratorView[]
            {
                new SwaggerInputView(Document.SwaggerGenerators.FromSwaggerCommand),
                new WebApiToSwaggerGeneratorView((WebApiToSwaggerCommand) Document.SwaggerGenerators.WebApiToSwaggerCommand),
                new JsonSchemaInputView(Document.SwaggerGenerators.JsonSchemaToSwaggerCommand),
                new AssemblyTypeToSwaggerGeneratorView((AssemblyTypeToSwaggerCommand) Document.SwaggerGenerators.AssemblyTypeToSwaggerCommand),
            };

            CodeGenerators = new ICodeGeneratorView[]
            {
                new SwaggerOutputView(),
                new SwaggerToTypeScriptClientGeneratorView(Document.CodeGenerators.SwaggerToTypeScriptClientCommand),
                new SwaggerToCSharpClientGeneratorView(Document.CodeGenerators.SwaggerToCSharpClientCommand),
                new SwaggerToCSharpControllerGeneratorView(Document.CodeGenerators.SwaggerToCSharpControllerCommand)
            };

            RaisePropertyChanged(() => SwaggerGenerators);
            RaisePropertyChanged(() => CodeGenerators);
        }

        public ISwaggerGeneratorView GetSwaggerGeneratorView()
        {
            return SwaggerGenerators.Single(g => g.Command == Document.SelectedSwaggerGenerator);
        }

        public string GetDocumentPath(ISwaggerGeneratorView generator)
        {
            return generator is SwaggerInputView && !string.IsNullOrEmpty(Document.SwaggerGenerators.FromSwaggerCommand.Url)
                ? Document.SwaggerGenerators.FromSwaggerCommand.Url
                : null;
        }
    }
}