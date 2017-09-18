//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
            SwaggerGenerators.FromSwaggerCommand = new FromSwaggerCommand();
            SwaggerGenerators.JsonSchemaToSwaggerCommand = new JsonSchemaToSwaggerCommand();

            SelectedSwaggerGenerator = SwaggerGenerators.FromSwaggerCommand;
        }

        /// <summary>Converts a path to an absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The absolute path.</returns>
        protected abstract string ConvertToAbsolutePath(string pathToConvert);

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected abstract string ConvertToRelativePath(string pathToConvert);

        /// <summary>Gets or sets the selected swagger generator JSON.</summary>
        [JsonProperty("SwaggerGenerator")]
        internal JObject SelectedSwaggerGeneratorRaw
        {
            get
            {
                var key = SelectedSwaggerGenerator.GetType().Name
                    .Replace("CommandBase", string.Empty)
                    .Replace("Command", string.Empty);

                return JObject.FromObject(new Dictionary<string, OutputCommandBase>
                {
                    {
                        key[0].ToString().ToLowerInvariant() + key.Substring(1),
                        SelectedSwaggerGenerator
                    }
                }, JsonSerializer.Create(GetSerializerSettings()));
            }
            set
            {
                var generatorProperty = value.Properties().First();
                var key = generatorProperty.Name + "Command";
                var collectionProperty = SwaggerGenerators.GetType().GetRuntimeProperty(key[0].ToString().ToUpperInvariant() + key.Substring(1));
                var generator = collectionProperty.GetValue(SwaggerGenerators);
                var newGenerator = (OutputCommandBase)JsonConvert.DeserializeObject(generatorProperty.Value.ToString(), generator.GetType(), GetSerializerSettings());
                collectionProperty.SetValue(SwaggerGenerators, newGenerator);
                SelectedSwaggerGenerator = newGenerator;
            }
        }

        /// <summary>Gets the swagger generators.</summary>
        [JsonIgnore]
        public SwaggerGeneratorCollection SwaggerGenerators { get; } = new SwaggerGeneratorCollection();

        /// <summary>Gets the code generators.</summary>
        [JsonProperty("CodeGenerators")]
        public CodeGeneratorCollection CodeGenerators { get; } = new CodeGeneratorCollection();

        /// <summary>Gets or sets the runtime where the document should be processed.</summary>
        public Runtime Runtime { get; set; }

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
        /// <param name="mappings">The mappings.</param>
        /// <returns>The document.</returns>
        protected static Task<TDocument> LoadAsync<TDocument>(string filePath, IDictionary<Type, Type> mappings)
            where TDocument : NSwagDocumentBase, new()
        {
            return Task.Run(async () =>
            {
                var saveFile = false;
                var data = await DynamicApis.FileReadAllTextAsync(filePath).ConfigureAwait(false);
                data = TransformLegacyDocument(data, out saveFile); // TODO: Remove this legacy stuff later

                var settings = GetSerializerSettings();
                settings.ContractResolver = new BaseTypeMappingContractResolver(mappings);

                var document = FromJson<TDocument>(filePath, data);

                if (saveFile)
                    await document.SaveAsync();

                return document;
            });
        }

        /// <summary>Converts the document to JSON.</summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="filePath">The file path.</param>
        /// <param name="data">The JSON data.</param>
        /// <returns>The document.</returns>
        public static TDocument FromJson<TDocument>(string filePath, string data)
            where TDocument : NSwagDocumentBase, new()
        {
            var settings = GetSerializerSettings();
            var document = JsonConvert.DeserializeObject<TDocument>(data, settings);
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());

            if (filePath != null)
            {
                document.Path = filePath;
                document.ConvertToAbsolutePaths();
            }

            return document;
        }

        /// <summary>Saves the document.</summary>
        /// <returns>The task.</returns>
        public Task SaveAsync()
        {
            return Task.Run(async () =>
            {
                await DynamicApis.FileWriteAllTextAsync(Path, ToJson()).ConfigureAwait(false);
                _latestData = JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
            });
        }

        /// <summary>Converts the document to JSON with relative paths.</summary>
        /// <returns>The JSON data.</returns>
        public string ToJsonWithRelativePaths()
        {
            ConvertToRelativePaths();
            try
            {
                return ToJson();
            }
            finally
            {
                ConvertToAbsolutePaths();
            }
        }

        /// <summary>Converts the document to JSON.</summary>
        /// <returns>The JSON data.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
        }

        /// <summary>Executes the document.</summary>
        /// <returns>The task.</returns>
        public async Task ExecuteAsync()
        {
            var document = await GenerateDocumentAsync();
            foreach (var codeGenerator in CodeGenerators.Items.Where(c => !string.IsNullOrEmpty(c.OutputFilePath)))
            {
                codeGenerator.Input = document;
                await codeGenerator.RunAsync(null, null);
                codeGenerator.Input = null;
            }
        }

        private async Task<SwaggerDocument> GenerateDocumentAsync()
        {
            return (SwaggerDocument)await SelectedSwaggerGenerator.RunAsync(null, null);
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            };
        }

        private void ConvertToAbsolutePaths()
        {
            if (SwaggerGenerators.FromSwaggerCommand != null)
            {
                if (!SwaggerGenerators.FromSwaggerCommand.Url.StartsWith("http://") && !SwaggerGenerators.FromSwaggerCommand.Url.StartsWith("https://"))
                    SwaggerGenerators.FromSwaggerCommand.Url = ConvertToAbsolutePath(SwaggerGenerators.FromSwaggerCommand.Url);
            }

            if (SwaggerGenerators.WebApiToSwaggerCommand != null)
            {
                SwaggerGenerators.WebApiToSwaggerCommand.DocumentTemplate = ConvertToAbsolutePath(SwaggerGenerators.WebApiToSwaggerCommand.DocumentTemplate);
                SwaggerGenerators.WebApiToSwaggerCommand.AssemblyPaths = SwaggerGenerators.WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
                SwaggerGenerators.WebApiToSwaggerCommand.ReferencePaths = SwaggerGenerators.WebApiToSwaggerCommand.ReferencePaths.Select(ConvertToAbsolutePath).ToArray();
                SwaggerGenerators.WebApiToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(SwaggerGenerators.WebApiToSwaggerCommand.AssemblyConfig);
            }

            if (SwaggerGenerators.AssemblyTypeToSwaggerCommand != null)
            {
                SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToAbsolutePath(SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyPath);
                SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToAbsolutePath(SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyConfig);
            }

            if (CodeGenerators.SwaggerToTypeScriptClientCommand != null)
            {
                CodeGenerators.SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(CodeGenerators.SwaggerToTypeScriptClientCommand.ExtensionCode);
            }

            if (CodeGenerators.SwaggerToCSharpClientCommand != null)
            {
                CodeGenerators.SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(CodeGenerators.SwaggerToCSharpClientCommand.ContractsOutputFilePath);
            }

            foreach (var generator in CodeGenerators.Items.Concat(SwaggerGenerators.Items))
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
        }

        private void ConvertToRelativePaths()
        {
            if (SwaggerGenerators.FromSwaggerCommand != null)
            {
                if (!SwaggerGenerators.FromSwaggerCommand.Url.StartsWith("http://") && !SwaggerGenerators.FromSwaggerCommand.Url.StartsWith("https://"))
                    SwaggerGenerators.FromSwaggerCommand.Url = ConvertToRelativePath(SwaggerGenerators.FromSwaggerCommand.Url);
            }

            if (SwaggerGenerators.WebApiToSwaggerCommand != null)
            {
                SwaggerGenerators.WebApiToSwaggerCommand.DocumentTemplate = ConvertToRelativePath(SwaggerGenerators.WebApiToSwaggerCommand.DocumentTemplate);
                SwaggerGenerators.WebApiToSwaggerCommand.AssemblyPaths = SwaggerGenerators.WebApiToSwaggerCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
                SwaggerGenerators.WebApiToSwaggerCommand.ReferencePaths = SwaggerGenerators.WebApiToSwaggerCommand.ReferencePaths.Select(ConvertToRelativePath).ToArray();
                SwaggerGenerators.WebApiToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(SwaggerGenerators.WebApiToSwaggerCommand.AssemblyConfig);
            }

            if (SwaggerGenerators.AssemblyTypeToSwaggerCommand != null)
            {
                SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToRelativePath(SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyPath);
                SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(SwaggerGenerators.AssemblyTypeToSwaggerCommand.AssemblyConfig);
            }

            if (CodeGenerators.SwaggerToTypeScriptClientCommand != null)
            {
                CodeGenerators.SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(CodeGenerators.SwaggerToTypeScriptClientCommand.ExtensionCode);
            }

            if (CodeGenerators.SwaggerToCSharpClientCommand != null)
            {
                CodeGenerators.SwaggerToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(CodeGenerators.SwaggerToCSharpClientCommand.ContractsOutputFilePath);
            }

            foreach (var generator in CodeGenerators.Items.Where(i => i != null).Concat(SwaggerGenerators.Items))
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

        private static string TransformLegacyDocument(string data, out bool saveFile)
        {
            saveFile = false;

            // New file format
            if (data.Contains("\"template\": \"Angular2\""))
            {
                data = data.Replace("\"template\": \"Angular2\"", "\"template\": \"Angular\"");
                saveFile = true;
            }

            if (data.Contains("\"SelectedSwaggerGenerator\""))
            {
                var obj = JsonConvert.DeserializeObject<JObject>(data);
                var selectedSwaggerGenerator = obj["SelectedSwaggerGenerator"].Value<int>();
                if (selectedSwaggerGenerator == 0) // swagger url/data
                {
                    obj["swaggerGenerator"] = new JObject
                    {
                        {
                            "fromSwagger", new JObject
                            {
                                {"url", obj["InputSwaggerUrl"]},
                                {"json", string.IsNullOrEmpty(obj["InputSwaggerUrl"].Value<string>()) ? obj["InputSwagger"] : null}
                            }
                        }
                    };
                }
                if (selectedSwaggerGenerator == 1) // web api
                {
                    obj["swaggerGenerator"] = new JObject
                    {
                        {"webApiToSwagger", obj["WebApiToSwaggerCommand"]}
                    };
                }
                if (selectedSwaggerGenerator == 2) // json schema
                {
                    obj["swaggerGenerator"] = new JObject
                    {
                        {
                            "jsonSchemaToSwagger", new JObject
                            {
                                {"schema", obj["InputJsonSchema"]},
                            }
                        }
                    };
                }
                if (selectedSwaggerGenerator == 3) // types
                {
                    obj["swaggerGenerator"] = new JObject
                    {
                        {"assemblyTypeToSwagger", obj["AssemblyTypeToSwaggerCommand"]}
                    };
                }

                obj["codeGenerators"] = new JObject
                {
                    {"swaggerToTypeScriptClient", obj["SwaggerToTypeScriptCommand"]},
                    {"swaggerToCSharpClient", obj["SwaggerToCSharpClientCommand"]},
                    {"swaggerToCSharpController", obj["SwaggerToCSharpControllerGenerator"]}
                };

                data = obj.ToString().Replace("\"OutputFilePath\"", "\"output\"");
                saveFile = true;
            }

            // typeScriptVersion

            if (data.Contains("generateReadOnlyKeywords") && !data.Contains("typeScriptVersion"))
            {
                data = data.Replace(@"""GenerateReadOnlyKeywords"": true", @"""typeScriptVersion"": 2.0");
                data = data.Replace(@"""generateReadOnlyKeywords"": true", @"""typeScriptVersion"": 2.0");

                data = data.Replace(@"""GenerateReadOnlyKeywords"": false", @"""typeScriptVersion"": 1.8");
                data = data.Replace(@"""generateReadOnlyKeywords"": false", @"""typeScriptVersion"": 1.8");

                saveFile = true;
            }

            // Full type names

            if (data.Contains("\"dateType\": \"DateTime\""))
            {
                data = data.Replace("\"dateType\": \"DateTime\"", "\"dateType\": \"System.DateTime\"");
                saveFile = true;
            }
            if (data.Contains("\"dateTimeType\": \"DateTime\""))
            {
                data = data.Replace("\"dateTimeType\": \"DateTime\"", "\"dateTimeType\": \"System.DateTime\"");
                saveFile = true;
            }
            if (data.Contains("\"timeType\": \"TimeSpan\""))
            {
                data = data.Replace("\"timeType\": \"TimeSpan\"", "\"timeType\": \"System.TimeSpan\"");
                saveFile = true;
            }
            if (data.Contains("\"timeSpanType\": \"TimeSpan\""))
            {
                data = data.Replace("\"timeSpanType\": \"TimeSpan\"", "\"timeSpanType\": \"System.TimeSpan\"");
                saveFile = true;
            }
            if (data.Contains("\"arrayType\": \"ObservableCollection\""))
            {
                data = data.Replace("\"arrayType\": \"ObservableCollection\"", "\"arrayType\": \"System.Collections.ObjectModel.ObservableCollection\"");
                saveFile = true;
            }
            if (data.Contains("\"dictionaryType\": \"Dictionary\""))
            {
                data = data.Replace("\"dictionaryType\": \"Dictionary\"", "\"dictionaryType\": \"System.Collections.Generic.Dictionary\"");
                saveFile = true;
            }

            return data;
        }
    }
}

