using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyToolkit.Model;
using NSwag.Commands;
using NSwag.Commands.SwaggerGeneration;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    public class DocumentModel : ObservableObject
    {
        public NSwagDocument Document { get; }

        /// <summary>Gets the swagger generators.</summary>
        public ISwaggerGeneratorView[] SwaggerGeneratorViews { get; }

        public IReadOnlyCollection<CodeGeneratorModel> CodeGenerators { get; }

        public IEnumerable<CodeGeneratorModel> SelectedCodeGenerators => CodeGenerators.Where(c => c.View.IsSelected);

        public DocumentModel(NSwagDocument document)
        {
            Document = document;

            SwaggerGeneratorViews = new ISwaggerGeneratorView[]
            {
                new SwaggerInputView(Document.SwaggerGenerators.FromSwaggerCommand),
                new WebApiToSwaggerGeneratorView(Document.SwaggerGenerators.WebApiToSwaggerCommand, document),
                new AspNetCoreToSwaggerGeneratorView(Document.SwaggerGenerators.AspNetCoreToSwaggerCommand, document),
                new JsonSchemaInputView(Document.SwaggerGenerators.JsonSchemaToSwaggerCommand),
                new AssemblyTypeToSwaggerGeneratorView(Document.SwaggerGenerators.TypesToSwaggerCommand, document),
            };

            CodeGenerators = new CodeGeneratorViewBase[]
            {
                new SwaggerOutputView(),
                new SwaggerToTypeScriptClientGeneratorView(Document),
                new SwaggerToCSharpClientGeneratorView(Document),
                new SwaggerToCSharpControllerGeneratorView(Document)
            }
            .Select(v => new CodeGeneratorModel { View = v })
            .ToList();

            foreach (var codeGenerator in CodeGenerators)
                codeGenerator.View.PropertyChanged += OnCodeGeneratorPropertyChanged;

            RaisePropertyChanged(() => SwaggerGeneratorViews);
            RaisePropertyChanged(() => CodeGenerators);
        }

        private void OnCodeGeneratorPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(CodeGeneratorViewBase.IsSelected))
                RaisePropertyChanged(() => SelectedCodeGenerators);
        }

        public ISwaggerGeneratorView GetSwaggerGeneratorView()
        {
            return SwaggerGeneratorViews.Single(g => g.Command == Document.SelectedSwaggerGenerator);
        }

        public string GetDocumentPath(ISwaggerGeneratorView generator)
        {
            return generator is SwaggerInputView && !string.IsNullOrEmpty(Document.SwaggerGenerators.FromSwaggerCommand.Url)
                ? Document.SwaggerGenerators.FromSwaggerCommand.Url
                : null;
        }
    }
}