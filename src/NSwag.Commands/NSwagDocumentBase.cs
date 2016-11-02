//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    /// <summary>The NSwagDocument base class.</summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class NSwagDocumentBase : INotifyPropertyChanged
    {
        private string _path;

        private int _selectedSwaggerGenerator;
        private int _selectedClientGenerator;

        private string _latestData;

        /// <summary>Initializes a new instance of the <see cref="NSwagDocumentBase"/> class.</summary>
        protected NSwagDocumentBase()
        {
            CodeGenerators.Add(SwaggerToTypeScriptClientCommand = new SwaggerToTypeScriptClientCommand());
            CodeGenerators.Add(SwaggerToCSharpClientCommand = new SwaggerToCSharpClientCommand());
            CodeGenerators.Add(SwaggerToCSharpControllerCommand = new SwaggerToCSharpControllerCommand());
        }

        /// <summary>Converts a path to an absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The absolute path.</returns>
        protected abstract string ConvertToAbsolutePath(string pathToConvert);

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected abstract string ConvertToRelativePath(string pathToConvert);

        /// <summary>Gets the swagger generators.</summary>
        protected List<OutputCommandBase> SwaggerGenerators { get; } = new List<OutputCommandBase>();
        
        /// <summary>Gets the code generators.</summary>
        protected List<InputOutputCommandBase> CodeGenerators { get; } = new List<InputOutputCommandBase>();

        /// <summary>Gets or sets the path.</summary>
        [JsonIgnore]
        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>Gets the name of the document.</summary>
        [JsonIgnore]
        public string Name => System.IO.Path.GetFileName(Path);

        /// <summary>Gets a value indicating whether the document is dirty (has any changes).</summary>
        [JsonIgnore]
        public bool IsDirty => _latestData != JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());

        /// <summary>Gets or sets the selected Swagger generator. </summary>
        [JsonProperty("SelectedSwaggerGenerator")]
        public int SelectedSwaggerGenerator
        {
            get { return _selectedSwaggerGenerator; }
            set
            {
                _selectedSwaggerGenerator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the selected client generator. </summary>
        [JsonIgnore]
        public int SelectedClientGenerator
        {
            get { return _selectedClientGenerator; }
            set
            {
                _selectedClientGenerator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the input Swagger specification.</summary>
        [JsonProperty("InputSwagger")]
        public string InputSwagger { get; set; }

        /// <summary>Gets or sets the input Swagger specification URL.</summary>
        [JsonProperty("InputSwaggerUrl")]
        public string InputSwaggerUrl { get; set; }

        /// <summary>Gets or sets the input JSON Schema.</summary>
        [JsonProperty("InputJsonSchema")]
        public string InputJsonSchema { get; set; }

        /// <summary>Gets or sets the WebApiToSwaggerCommand.</summary>
        [JsonProperty("WebApiToSwaggerCommand")]
        public WebApiToSwaggerCommandBase WebApiToSwaggerCommand { get; set; }

        /// <summary>Gets or sets the AssemblyTypeToSwaggerCommand.</summary>
        [JsonProperty("AssemblyTypeToSwaggerCommand")]
        public AssemblyTypeToSwaggerCommandBase AssemblyTypeToSwaggerCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonProperty("SwaggerToTypeScriptCommand")]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonProperty("SwaggerToCSharpClientCommand")]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonProperty("SwaggerToCSharpControllerCommand")]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand { get; set; }

        /// <summary>Creates a new NSwagDocument.</summary>
        /// <typeparam name="TDocument">The type.</typeparam>
        /// <returns>The document.</returns>
        protected static TDocument Create<TDocument>()
            where TDocument : NSwagDocumentBase, new()
        {
            var document = new TDocument();
            document.Path = "Untitled";
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());
            return document;
        }

        /// <summary>Loads an existing NSwagDocument.</summary>
        /// <typeparam name="TDocument">The type.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <returns>The document.</returns>
        protected static Task<TDocument> LoadAsync<TDocument>(string filePath)
            where TDocument : NSwagDocumentBase, new()
        {
            return Task.Run(() =>
            {
                var data = DynamicApis.FileReadAllText(filePath);

                var document = JsonConvert.DeserializeObject<TDocument>(data);
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

        /// <summary>Saves the document.</summary>
        /// <returns>The task.</returns>
        public Task SaveAsync()
        {
            return Task.Run(() =>
            {
                ConvertToRelativePaths();

                _latestData = JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
                ConvertToAbsolutePaths();
                DynamicApis.FileWriteAllText(Path, _latestData);
            });
        }

        /// <summary>Executes the document.</summary>
        /// <returns>The task.</returns>
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

        /// <summary>Generates the Swagger specification.</summary>
        /// <returns>The Swagger specification.</returns>
        protected virtual Task<SwaggerService> GenerateServiceAsync()
        {
            // TODO: Add a command for all 4 input types, then just call RunAsync on the selected generator

            if (SelectedSwaggerGenerator == 0)
            {
                if (!string.IsNullOrEmpty(InputSwaggerUrl))
                    return Task.FromResult(SwaggerService.FromUrl(InputSwaggerUrl));
                else
                    return Task.FromResult(SwaggerService.FromJson(InputSwagger));
            }
            else if (SelectedSwaggerGenerator == 2)
            {
                var schema = JsonSchema4.FromJson(InputJsonSchema);
                var service = new SwaggerService();
                service.Definitions[schema.TypeNameRaw ?? "MyType"] = schema;
                return Task.FromResult(service);
            }

            return null;
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

        private void Loaded()
        {
            WebApiToSwaggerCommand.ControllerName = "";
        }

        private void ConvertToAbsolutePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToAbsolutePath(WebApiToSwaggerCommand.DocumentTemplate);
            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
            WebApiToSwaggerCommand.ReferencePaths = WebApiToSwaggerCommand.ReferencePaths.Select(ConvertToAbsolutePath).ToArray();
            WebApiToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(WebApiToSwaggerCommand.AssemblyConfig);

            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyPath);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
        }

        private void ConvertToRelativePaths()
        {
            WebApiToSwaggerCommand.DocumentTemplate = ConvertToRelativePath(WebApiToSwaggerCommand.DocumentTemplate);
            WebApiToSwaggerCommand.AssemblyPaths = WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
            WebApiToSwaggerCommand.ReferencePaths = WebApiToSwaggerCommand.ReferencePaths.Select(ConvertToRelativePath).ToArray();
            WebApiToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(WebApiToSwaggerCommand.AssemblyConfig);

            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyPath);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Concat(SwaggerGenerators))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Raises all properties changed.</summary>
        public void RaiseAllPropertiesChanged()
        {
            OnPropertyChanged(null);
        }
    }
}
