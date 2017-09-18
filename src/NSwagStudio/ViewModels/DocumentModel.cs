using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Model;
using NSwag.Commands;
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
                new WebApiToSwaggerGeneratorView((WebApiToSwaggerCommand) Document.SwaggerGenerators.WebApiToSwaggerCommand),
                new JsonSchemaInputView(Document.SwaggerGenerators.JsonSchemaToSwaggerCommand),
                new AssemblyTypeToSwaggerGeneratorView((AssemblyTypeToSwaggerCommand) Document.SwaggerGenerators.AssemblyTypeToSwaggerCommand),
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

        public async Task<string> ExecuteCommandLineAsync()
        {
            return await Task.Run(async () =>
            {
                if (Document.Runtime == Runtime.Debug)
                {
                    await Document.ExecuteAsync();
                    return string.Empty;
                }

                var name = Path.GetTempPath() + "nswag_document_" + Guid.NewGuid();

                File.WriteAllText(name, this.Document.ToJson());
                try
                {
                    var processStart = new ProcessStartInfo(GetProgramName(), GetArgumentsPrefix() + "run \"" + name + "\"");
                    processStart.RedirectStandardOutput = true;
                    processStart.RedirectStandardError = true;
                    processStart.UseShellExecute = false;
                    processStart.CreateNoWindow = true;
                    processStart.WindowStyle = ProcessWindowStyle.Hidden;

                    var process = Process.Start(processStart);
                    var output = await process.StandardOutput.ReadToEndAsync();

                    if (process.ExitCode != 0)
                    {
                        var error = await process.StandardError.ReadToEndAsync();
                        if (error != null)
                            throw new InvalidOperationException(output + error);
                    }

                    return output;
                }
                finally
                {
                    File.Delete(name);
                }
            });
        }

        private string GetArgumentsPrefix()
        {
            var applicationDirectory = Path.GetDirectoryName(typeof(DocumentModel).Assembly.Location);

            if (Document.Runtime == Runtime.Core10)
                return "\"" + Path.Combine(applicationDirectory, "netcoreapp1.0/dotnet-nswag.dll") + "\" ";
            else if (Document.Runtime == Runtime.Core11)
                return "\"" + Path.Combine(applicationDirectory, "netcoreapp1.1/dotnet-nswag.dll") + "\" ";
            else if (Document.Runtime == Runtime.Core20)
                return "\"" + Path.Combine(applicationDirectory, "netcoreapp2.0/dotnet-nswag.dll") + "\" ";
            else
                return "";
        }

        private string GetProgramName()
        {
            var applicationDirectory = Path.GetDirectoryName(typeof(DocumentModel).Assembly.Location);

            if (Document.Runtime == Runtime.WinX64)
                return Path.Combine(applicationDirectory, "full/nswag.exe");
            else if (Document.Runtime == Runtime.WinX86)
                return Path.Combine(applicationDirectory, "full/nswag.x86.exe");
            else
                return "dotnet";
        }
    }
}