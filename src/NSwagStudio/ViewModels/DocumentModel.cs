using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using MyToolkit.Model;
using NSwag.Commands;
using NSwagStudio.Views.CodeGenerators;
using NSwagStudio.Views.SwaggerGenerators;

namespace NSwagStudio.ViewModels
{
    public class CodeGeneratorModel : ObservableObject
    {
        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(ref _isSelected, value); }
        }

        public bool IsPersistent => View is SwaggerOutputView;

        public ICodeGeneratorView View { get; set; }
    }

    public class DocumentModel : ObservableObject
    {
        public NSwagDocument Document { get; }

        /// <summary>Gets the swagger generators.</summary>
        public ISwaggerGeneratorView[] SwaggerGeneratorViews { get; }

        public IReadOnlyCollection<CodeGeneratorModel> CodeGenerators { get; }

        public IEnumerable<CodeGeneratorModel> SelectedCodeGenerators => CodeGenerators.Where(c => c.IsSelected);

        public DocumentModel(NSwagDocument document)
        {
            Document = document;

            SwaggerGeneratorViews = new ISwaggerGeneratorView[]
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
            }.Select(v => new CodeGeneratorModel
            {
                View = v
            }).ToList();
            CodeGenerators.First().IsSelected = true; 

            foreach (var codeGenerator in CodeGenerators)
                codeGenerator.PropertyChanged += OnCodeGeneratorPropertyChanged;

            RaisePropertyChanged(() => SwaggerGeneratorViews);
            RaisePropertyChanged(() => CodeGenerators);
        }

        private void OnCodeGeneratorPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(CodeGeneratorModel.IsSelected))
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