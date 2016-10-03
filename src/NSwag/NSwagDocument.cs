//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyToolkit.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag.Commands;
using NSwag.Commands.Base;
using NSwag.CodeGeneration.Utilities;

namespace NSwag
{
    public class NSwagDocument : ObservableObject
    {
        private readonly List<OutputCommandBase> _swaggerGenerators = new List<OutputCommandBase>();
        private readonly List<InputOutputCommandBase> _codeGenerators = new List<InputOutputCommandBase>();

        private string _path;

        private int _selectedSwaggerGenerator;
        private int _selectedClientGenerator;

        private string _latestData;

        public NSwagDocument()
        {
            _swaggerGenerators.Add(WebApiToSwaggerCommand = new WebApiToSwaggerCommand());
            _swaggerGenerators.Add(AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand());

            _codeGenerators.Add(SwaggerToTypeScriptClientCommand = new SwaggerToTypeScriptClientCommand());
            _codeGenerators.Add(SwaggerToCSharpClientCommand = new SwaggerToCSharpClientCommand());
            _codeGenerators.Add(SwaggerToCSharpControllerCommand = new SwaggerToCSharpControllerCommand());
        }

        [JsonIgnore]
        public string Path
        {
            get { return _path; }
            set
            {
                if (Set(ref _path, value))
                    RaisePropertyChanged(() => Name);
            }
        }

        [JsonIgnore]
        public string Name => System.IO.Path.GetFileName(Path);

        [JsonIgnore]
        public bool IsDirty => _latestData != JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());

        /// <summary>Gets or sets the selected Swagger generator. </summary>
        [JsonProperty("SelectedSwaggerGenerator")]
        public int SelectedSwaggerGenerator
        {
            get { return _selectedSwaggerGenerator; }
            set { Set(ref _selectedSwaggerGenerator, value); }
        }

        /// <summary>Gets or sets the selected client generator. </summary>
        [JsonIgnore]
        public int SelectedClientGenerator
        {
            get { return _selectedClientGenerator; }
            set { Set(ref _selectedClientGenerator, value); }
        }

        [JsonProperty("InputSwagger")]
        public string InputSwagger { get; set; }

        [JsonProperty("InputSwaggerUrl")]
        public string InputSwaggerUrl { get; set; }

        [JsonProperty("InputJsonSchema")]
        public string InputJsonSchema { get; set; }

        [JsonProperty("WebApiToSwaggerCommand")]
        public WebApiToSwaggerCommand WebApiToSwaggerCommand { get; set; }

        [JsonProperty("AssemblyTypeToSwaggerCommand")]
        public AssemblyTypeToSwaggerCommand AssemblyTypeToSwaggerCommand { get; set; }

        [JsonProperty("SwaggerToTypeScriptCommand")]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand { get; set; }

        [JsonProperty("SwaggerToCSharpClientCommand")]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand { get; set; }

        [JsonProperty("SwaggerToCSharpControllerGenerator")]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand { get; set; }

        public static NSwagDocument Create()
        {
            var document = new NSwagDocument();
            document.Path = "Untitled";
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());
            return document;
        }

        public static Task<NSwagDocument> LoadAsync(string filePath)
        {
            return Task.Run(() =>
            {
                var data = File.ReadAllText(filePath);

                var document = JsonConvert.DeserializeObject<NSwagDocument>(data);
                document.Path = filePath;
                document.ConvertToAbsolutePaths();

                document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented,
                    GetSerializerSettings());

                // Legacy file support
                if (document.SwaggerToCSharpClientCommand.DateTimeType == "0")
                    document.SwaggerToCSharpClientCommand.DateTimeType = "DateTime";
                document.WebApiToSwaggerCommand.ControllerName = "";

                return document;
            });
        }

        public Task SaveAsync()
        {
            return Task.Run(() =>
            {
                ConvertToRelativePaths();

                _latestData = JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
                ConvertToAbsolutePaths();
                File.WriteAllText(Path, _latestData);
            });
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            };
        }

        public async Task ExecuteAsync()
        {
            SwaggerService service = null;
            foreach (var codeGenerator in _codeGenerators)
            {
                if (!string.IsNullOrEmpty(codeGenerator.OutputFilePath))
                {
                    if (service == null)
                        service = await GenerateServiceAsync();

                    codeGenerator.Input = service;
                    await codeGenerator.RunAsync(null, null);
                    codeGenerator.Input = null;
                }
            }
        }

        private async Task<SwaggerService> GenerateServiceAsync()
        {
            // TODO: Add a command for all 4 input types, then just call RunAsync on the selected generator

            if (SelectedSwaggerGenerator == 0)
            {
                if (!string.IsNullOrEmpty(InputSwaggerUrl))
                    return SwaggerService.FromUrl(InputSwaggerUrl);
                else
                    return SwaggerService.FromJson(InputSwagger);
            }
            else if (SelectedSwaggerGenerator == 1)
                return await WebApiToSwaggerCommand.RunAsync();
            else if (SelectedSwaggerGenerator == 2)
            {
                var schema = JsonSchema4.FromJson(InputJsonSchema);
                var service = new SwaggerService();
                service.Definitions[schema.TypeNameRaw ?? "MyType"] = schema;
                return service;
            }
            else
                return await AssemblyTypeToSwaggerCommand.RunAsync();
        }

        private void ConvertToAbsolutePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToAbsolutePath(WebApiToSwaggerCommand.DocumentTemplate);

            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyPath);

            WebApiToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in _codeGenerators.Concat(_swaggerGenerators))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
        }

        private void ConvertToRelativePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToRelativePath(WebApiToSwaggerCommand.DocumentTemplate);

            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyPath);

            WebApiToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in _codeGenerators.Concat(_swaggerGenerators))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);
        }

        private string ConvertToAbsolutePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !System.IO.Path.IsPathRooted(pathToConvert))
                return PathUtilities.MakeAbsolutePath(pathToConvert, System.IO.Path.GetDirectoryName(Path));
            return pathToConvert;
        }

        private string ConvertToRelativePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert))
                return PathUtilities.MakeRelativePath(pathToConvert, System.IO.Path.GetDirectoryName(Path));
            return pathToConvert;
        }
    }
}
