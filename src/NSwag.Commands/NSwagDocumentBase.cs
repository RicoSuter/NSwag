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
using Newtonsoft.Json.Linq;
using NJsonSchema.Infrastructure;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    /// <summary>The NSwagDocument base class.</summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class NSwagDocumentBase : INotifyPropertyChanged
    {
        private string _path;
        private string _latestData;
        private OutputCommandBase _selectedSwaggerGenerator;

        /// <summary>Initializes a new instance of the <see cref="NSwagDocumentBase"/> class.</summary>
        protected NSwagDocumentBase()
        {
            AddSwaggerGenerator(new JsonSchemaToSwaggerCommand());
            AddSwaggerGenerator(new InputToSwaggerCommand());

            AddCodeGenerator(new SwaggerToTypeScriptClientCommand());
            AddCodeGenerator(new SwaggerToCSharpClientCommand());
            AddCodeGenerator(new SwaggerToCSharpControllerCommand());

            SelectedSwaggerGenerator = SwaggerGenerators.First().Value;
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
        [JsonIgnore]
        internal Dictionary<string, OutputCommandBase> SwaggerGenerators { get; } = new Dictionary<string, OutputCommandBase>();

        /// <summary>Gets the code generators.</summary>
        [JsonProperty("CodeGenerators")]
        internal Dictionary<string, InputOutputCommandBase> CodeGenerators { get; } = new Dictionary<string, InputOutputCommandBase>();

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

        [JsonProperty("SwaggerGenerator")]
        internal object SelectedSwaggerGeneratorRaw
        {
            get
            {
                return new Dictionary<string, OutputCommandBase>
                {
                    { SwaggerGenerators.Single(p => p.Value == SelectedSwaggerGenerator).Key, SelectedSwaggerGenerator }
                };
            }
            set
            {
                var obj = (JObject)value;
                var generatorProperty = obj.Properties().First();
                var generator = SwaggerGenerators[generatorProperty.Name];
                var newGenerator = (OutputCommandBase)JsonConvert.DeserializeObject(generatorProperty.Value.ToString(), generator.GetType());
                SwaggerGenerators[generatorProperty.Name] = newGenerator;
                SelectedSwaggerGenerator = newGenerator;
            }
        }

        /// <summary>Gets the selected Swagger generator.</summary>
        [JsonIgnore]
        public OutputCommandBase SelectedSwaggerGenerator
        {
            get { return _selectedSwaggerGenerator; }
            set
            {
                _selectedSwaggerGenerator = value;
                OnPropertyChanged();
            }
        }

        /// <summary>Gets or sets the input to swagger command.</summary>
        [JsonIgnore]
        public InputToSwaggerCommand InputToSwaggerCommand
            => GetSwaggerGenerator<InputToSwaggerCommand>();

        /// <summary>Gets or sets the json schema to swagger command.</summary>
        [JsonIgnore]
        public JsonSchemaToSwaggerCommand JsonSchemaToSwaggerCommand
            => GetSwaggerGenerator<JsonSchemaToSwaggerCommand>();

        /// <summary>Gets or sets the WebApiToSwaggerCommand.</summary>
        [JsonIgnore]
        public WebApiToSwaggerCommandBase WebApiToSwaggerCommand
            => GetSwaggerGenerator<WebApiToSwaggerCommandBase>();

        /// <summary>Gets or sets the AssemblyTypeToSwaggerCommand.</summary>
        [JsonIgnore]
        public AssemblyTypeToSwaggerCommandBase AssemblyTypeToSwaggerCommand
            => GetSwaggerGenerator<AssemblyTypeToSwaggerCommandBase>();

        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonIgnore]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand
            => GetCodeGenerator<SwaggerToTypeScriptClientCommand>();

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonIgnore]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand
            => GetCodeGenerator<SwaggerToCSharpClientCommand>();

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonIgnore]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand
            => GetCodeGenerator<SwaggerToCSharpControllerCommand>();

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

        /// <summary>Adds the swagger generator.</summary>
        /// <param name="command">The command.</param>
        protected void AddSwaggerGenerator(OutputCommandBase command)
        {
            SwaggerGenerators.Add(command.GetType().Name.Replace("CommandBase", string.Empty).Replace("Command", string.Empty), command);
        }

        /// <summary>Gets the code generator.</summary>
        protected T GetSwaggerGenerator<T>()
            where T : OutputCommandBase
        {
            return (T)SwaggerGenerators[typeof(T).Name.Replace("CommandBase", string.Empty).Replace("Command", string.Empty)];
        }

        /// <summary>Adds the code generator.</summary>
        /// <param name="command">The command.</param>
        protected void AddCodeGenerator(InputOutputCommandBase command)
        {
            CodeGenerators.Add(command.GetType().Name.Replace("CommandBase", string.Empty).Replace("Command", string.Empty), command);
        }

        /// <summary>Gets the code generator.</summary>
        protected T GetCodeGenerator<T>()
            where T : InputOutputCommandBase
        {
            return (T)CodeGenerators[typeof(T).Name.Replace("CommandBase", string.Empty).Replace("Command", string.Empty)];
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
            SwaggerDocument document = null;
            foreach (var codeGenerator in CodeGenerators)
            {
                if (!string.IsNullOrEmpty(codeGenerator.Value.OutputFilePath))
                {
                    if (document == null)
                        document = await GenerateDocumentAsync();

                    codeGenerator.Value.Input = document;
                    await codeGenerator.Value.RunAsync(null, null);
                    codeGenerator.Value.Input = null;
                }
            }
        }

        private async Task<SwaggerDocument> GenerateDocumentAsync()
        {
            return await ((dynamic)SelectedSwaggerGenerator).RunAsync();
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

            foreach (var generator in CodeGenerators.Values.Concat(SwaggerGenerators.Values))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Values.Concat(SwaggerGenerators.Values))
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

            foreach (var generator in CodeGenerators.Values.Concat(SwaggerGenerators.Values))
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);
            SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.ContractsOutputFilePath);

            foreach (var generator in CodeGenerators.Values.Concat(SwaggerGenerators.Values))
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

