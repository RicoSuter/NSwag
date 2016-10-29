using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSwag.CodeGeneration.Utilities;
using NSwag.Commands;

namespace NSwag
{
    public class NSwagDocument : NSwagDocumentBase
    {
        public NSwagDocument()
        {
            SwaggerGenerators.Add(WebApiToSwaggerCommand = new WebApiToSwaggerCommand());
            SwaggerGenerators.Add(AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand());
        }

        public static NSwagDocument Create()
        {
            return Create<NSwagDocument>();
        }

        public static Task<NSwagDocument> LoadAsync(string filePath)
        {
            return LoadAsync<NSwagDocument>(filePath);
        }

        [JsonProperty("WebApiToSwaggerCommand")]
        public WebApiToSwaggerCommand WebApiToSwaggerCommand { get; set; }

        [JsonProperty("AssemblyTypeToSwaggerCommand")]
        public AssemblyTypeToSwaggerCommand AssemblyTypeToSwaggerCommand { get; set; }

        protected override void ConvertToAbsolutePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToAbsolutePath(WebApiToSwaggerCommand.DocumentTemplate);

            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyPath);

            WebApiToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
        }

        protected override void ConvertToRelativePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToRelativePath(WebApiToSwaggerCommand.DocumentTemplate);

            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyPath);

            WebApiToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);
        }

        protected override string ConvertToAbsolutePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !System.IO.Path.IsPathRooted(pathToConvert))
                return PathUtilities.MakeAbsolutePath(pathToConvert, System.IO.Path.GetDirectoryName(Path));
            return pathToConvert;
        }

        protected override string ConvertToRelativePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert))
                return PathUtilities.MakeRelativePath(pathToConvert, System.IO.Path.GetDirectoryName(Path));
            return pathToConvert;
        }

        protected override void Loaded()
        {
            WebApiToSwaggerCommand.ControllerName = "";
        }

        protected override async Task<SwaggerService> GenerateServiceAsync()
        {
            if (SelectedSwaggerGenerator == 1)
                return await WebApiToSwaggerCommand.RunAsync();
            else if (SelectedSwaggerGenerator == 3)
                return await AssemblyTypeToSwaggerCommand.RunAsync();
            else
                return await base.GenerateServiceAsync();
        }
    }
}