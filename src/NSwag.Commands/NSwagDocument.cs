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
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    public abstract class NSwagDocumentBase : ObservableObject
    {
        private string _path;

        private int _selectedSwaggerGenerator;
        private int _selectedClientGenerator;

        private string _latestData;

        protected NSwagDocumentBase()
        {
            CodeGenerators.Add(SwaggerToTypeScriptClientCommand = new SwaggerToTypeScriptClientCommand());
            CodeGenerators.Add(SwaggerToCSharpClientCommand = new SwaggerToCSharpClientCommand());
            CodeGenerators.Add(SwaggerToCSharpControllerCommand = new SwaggerToCSharpControllerCommand());
        }

        protected List<OutputCommandBase> SwaggerGenerators { get; } = new List<OutputCommandBase>();

        protected List<InputOutputCommandBase> CodeGenerators { get; } = new List<InputOutputCommandBase>();

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

        [JsonProperty("SwaggerToTypeScriptCommand")]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand { get; set; }

        [JsonProperty("SwaggerToCSharpClientCommand")]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand { get; set; }

        [JsonProperty("SwaggerToCSharpControllerGenerator")]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand { get; set; }

        protected static T Create<T>()
            where T : NSwagDocumentBase, new()
        {
            var document = new T();
            document.Path = "Untitled";
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());
            return document;
        }

        protected static Task<T> LoadAsync<T>(string filePath)
            where T : NSwagDocumentBase, new()
        {
            return Task.Run(() =>
            {
                var data = File.ReadAllText(filePath);

                var document = JsonConvert.DeserializeObject<T>(data);
                document.Path = filePath;
                document.ConvertToAbsolutePaths();

                document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented,
                    GetSerializerSettings());

                // Legacy file support
                if (document.SwaggerToCSharpClientCommand.DateTimeType == "0")
                    document.SwaggerToCSharpClientCommand.DateTimeType = "DateTime";

                document.Loaded();
                return document;
            });
        }

        protected abstract void Loaded();

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
            foreach (var codeGenerator in CodeGenerators)
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

        protected virtual async Task<SwaggerService> GenerateServiceAsync()
        {
            // TODO: Add a command for all 4 input types, then just call RunAsync on the selected generator

            if (SelectedSwaggerGenerator == 0)
            {
                if (!string.IsNullOrEmpty(InputSwaggerUrl))
                    return SwaggerService.FromUrl(InputSwaggerUrl);
                else
                    return SwaggerService.FromJson(InputSwagger);
            }
            else if (SelectedSwaggerGenerator == 2)
            {
                var schema = JsonSchema4.FromJson(InputJsonSchema);
                var service = new SwaggerService();
                service.Definitions[schema.TypeNameRaw ?? "MyType"] = schema;
                return service;
            }

            return null;
        }

        protected virtual void ConvertToAbsolutePaths()
        {
            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
        }

        protected virtual void ConvertToRelativePaths()
        {
            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);
        }

        protected abstract string ConvertToAbsolutePath(string pathToConvert);

        protected abstract string ConvertToRelativePath(string pathToConvert);
    }
}
