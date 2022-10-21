//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NJsonSchema.Infrastructure;
using NSwag.Commands.CodeGeneration;
using NSwag.Commands.Generation;

namespace NSwag.Commands
{
    /// <summary>The NSwagDocument base class.</summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class NSwagDocumentBase : INotifyPropertyChanged
    {
        private string _path;
        private string _latestData;
        private IOutputCommand _selectedSwaggerGenerator;

        /// <summary>Initializes a new instance of the <see cref="NSwagDocumentBase"/> class.</summary>
        protected NSwagDocumentBase()
        {
            SwaggerGenerators.FromDocumentCommand = new FromDocumentCommand();
            SwaggerGenerators.JsonSchemaToOpenApiCommand = new JsonSchemaToOpenApiCommand();

            SelectedSwaggerGenerator = SwaggerGenerators.FromDocumentCommand;
        }

        /// <summary>Converts a path to an absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The absolute path.</returns>
        protected abstract string ConvertToAbsolutePath(string pathToConvert);

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected abstract string ConvertToRelativePath(string pathToConvert);

        /// <summary>Executes the current document.</summary>
        /// <returns>The result.</returns>
        public abstract Task<OpenApiDocumentExecutionResult> ExecuteAsync();

        /// <summary>Gets or sets the runtime where the document should be processed.</summary>
        public Runtime Runtime { get; set; } = Runtime.NetCore21;

        /// <summary>Gets or sets the default variables.</summary>
        public string DefaultVariables { get; set; }

        /// <summary>Gets or sets the selected swagger generator JSON.</summary>
        [JsonProperty("DocumentGenerator")]
        internal JObject SelectedSwaggerGeneratorRaw
        {
            get
            {
                var key = SelectedSwaggerGenerator.GetType().Name
                    .Replace("CommandBase", string.Empty)
                    .Replace("Command", string.Empty);

                return JObject.FromObject(new Dictionary<string, IOutputCommand>
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
                var newGenerator = (IOutputCommand)JsonConvert.DeserializeObject(generatorProperty.Value.ToString(), generator.GetType(), GetSerializerSettings());
                collectionProperty.SetValue(SwaggerGenerators, newGenerator);
                SelectedSwaggerGenerator = newGenerator;
            }
        }

        /// <summary>Gets the swagger generators.</summary>
        [JsonIgnore]
        public OpenApiGeneratorCollection SwaggerGenerators { get; } = new OpenApiGeneratorCollection();

        /// <summary>Gets the code generators.</summary>
        [JsonProperty("CodeGenerators")]
        public CodeGeneratorCollection CodeGenerators { get; } = new CodeGeneratorCollection();

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
        public string Name
        {
            get
            {
                var name = System.IO.Path.GetFileName(Path);
                if (!name.Equals("nswag.json", StringComparison.OrdinalIgnoreCase))
                {
                    return name;
                }

                var segments = Path.Replace("\\", "/").Split('/');
                return segments.Length >= 2 ? string.Join("/", segments.Skip(segments.Length - 2)) : name;
            }
        }

        /// <summary>Gets a value indicating whether the document is dirty (has any changes).</summary>
        [JsonIgnore]
        public bool IsDirty => _latestData != JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());

        /// <summary>Gets the selected Swagger generator.</summary>
        [JsonIgnore]
        public IOutputCommand SelectedSwaggerGenerator
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
        /// <param name="variables">The variables.</param>
        /// <param name="applyTransformations">Specifies whether to expand environment variables and convert variables.</param>
        /// <param name="mappings">The mappings.</param>
        /// <returns>The document.</returns>
        protected static async Task<TDocument> LoadAsync<TDocument>(
            string filePath,
            string variables,
            bool applyTransformations,
            IDictionary<Type, Type> mappings)
            where TDocument : NSwagDocumentBase, new()
        {
            var data = DynamicApis.FileReadAllText(filePath);
            data = TransformLegacyDocument(data, out var requiredLegacyTransformations);

            if (requiredLegacyTransformations)
            {
                // Save now to avoid transformations
                var document = LoadDocument<TDocument>(filePath, mappings, data);
                await document.SaveAsync();
            }

            if (applyTransformations)
            {
                data = Regex.Replace(data, "%[A-Za-z0-9_]*?%", p => EscapeJsonString(Environment.ExpandEnvironmentVariables(p.Value)));

                foreach (var p in ConvertVariables(variables))
                {
                    data = data.Replace("$(" + p.Key + ")", EscapeJsonString(p.Value));
                }

                var obj = JObject.Parse(data);
                if (obj["defaultVariables"] != null)
                {
                    var defaultVariables = obj["defaultVariables"].Value<string>();
                    foreach (var p in ConvertVariables(defaultVariables))
                    {
                        data = data.Replace("$(" + p.Key + ")", EscapeJsonString(p.Value));
                    }
                }
            }

            return LoadDocument<TDocument>(filePath, mappings, data);
        }

        private static TDocument LoadDocument<TDocument>(string filePath, IDictionary<Type, Type> mappings, string data)
            where TDocument : NSwagDocumentBase, new()
        {
            var settings = GetSerializerSettings();
            settings.ContractResolver = new BaseTypeMappingContractResolver(mappings);

            var document = FromJson<TDocument>(filePath, data);
            return document;
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

            if (filePath != null)
            {
                document.Path = filePath;
                document.ConvertToAbsolutePaths();
            }

            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());

            return document;
        }

        /// <summary>Saves the document.</summary>
        /// <returns>The task.</returns>
        public Task SaveAsync()
        {
            DynamicApis.FileWriteAllText(Path, ToJsonWithRelativePaths());
            _latestData = JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
            return Task.CompletedTask;
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

        /// <summary>Generates the <see cref="OpenApiDocument"/> with the currently selected generator.</summary>
        /// <returns>The document.</returns>
        protected async Task<OpenApiDocument> GenerateSwaggerDocumentAsync()
        {
            return (OpenApiDocument) await SelectedSwaggerGenerator.RunAsync(null, null);
        }

        private static string EscapeJsonString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = JsonConvert.ToString(value);
                return value.Substring(1, value.Length - 2);
            }

            return string.Empty;
        }

        private static Dictionary<string, string> ConvertVariables(string variables)
        {
            try
            {
                return (variables ?? "")
                    .Split(',').Where(p => !string.IsNullOrEmpty(p))
                    .ToDictionary(p => p.Split('=')[0], p => p.Split('=')[1]);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Could not parse variables, ensure that they are " +
                                                    "in the form 'key1=value1,key2=value2', variables: " + variables, exception);
            }
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>
                {
                    new StringEnumConverter()
                }
            };
        }

        private void ConvertToAbsolutePaths()
        {
            if (SwaggerGenerators.FromDocumentCommand != null)
            {
                if (!SwaggerGenerators.FromDocumentCommand.Url.StartsWith("http://") && !SwaggerGenerators.FromDocumentCommand.Url.StartsWith("https://"))
                {
                    SwaggerGenerators.FromDocumentCommand.Url = ConvertToAbsolutePath(SwaggerGenerators.FromDocumentCommand.Url);
                }
            }

            if (SwaggerGenerators.WebApiToOpenApiCommand != null)
            {
                SwaggerGenerators.WebApiToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.WebApiToOpenApiCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
                SwaggerGenerators.WebApiToOpenApiCommand.ReferencePaths =
                    SwaggerGenerators.WebApiToOpenApiCommand.ReferencePaths.Select(ConvertToAbsolutePath).ToArray();

                SwaggerGenerators.WebApiToOpenApiCommand.DocumentTemplate = ConvertToAbsolutePath(
                    SwaggerGenerators.WebApiToOpenApiCommand.DocumentTemplate);
                SwaggerGenerators.WebApiToOpenApiCommand.AssemblyConfig = ConvertToAbsolutePath(
                    SwaggerGenerators.WebApiToOpenApiCommand.AssemblyConfig);
            }

            if (SwaggerGenerators.AspNetCoreToOpenApiCommand != null)
            {
                SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
                SwaggerGenerators.AspNetCoreToOpenApiCommand.ReferencePaths =
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.ReferencePaths.Select(ConvertToAbsolutePath).ToArray();

                SwaggerGenerators.AspNetCoreToOpenApiCommand.DocumentTemplate = ConvertToAbsolutePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.DocumentTemplate);
                SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyConfig = ConvertToAbsolutePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyConfig);

                SwaggerGenerators.AspNetCoreToOpenApiCommand.Project = ConvertToAbsolutePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.Project);
                SwaggerGenerators.AspNetCoreToOpenApiCommand.MSBuildProjectExtensionsPath = ConvertToAbsolutePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.MSBuildProjectExtensionsPath);

                SwaggerGenerators.AspNetCoreToOpenApiCommand.WorkingDirectory = ConvertToAbsolutePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.WorkingDirectory);
            }

            if (SwaggerGenerators.TypesToOpenApiCommand != null)
            {
                SwaggerGenerators.TypesToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.TypesToOpenApiCommand.AssemblyPaths.Select(ConvertToAbsolutePath).ToArray();
                SwaggerGenerators.TypesToOpenApiCommand.AssemblyConfig = ConvertToAbsolutePath(
                    SwaggerGenerators.TypesToOpenApiCommand.AssemblyConfig);
            }

            if (CodeGenerators.OpenApiToTypeScriptClientCommand != null)
            {
                CodeGenerators.OpenApiToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolutePath(
                    CodeGenerators.OpenApiToTypeScriptClientCommand.ExtensionCode);
                CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory = ConvertToAbsolutePath(
                    CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory);

                if (CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory != null && !Directory.Exists(CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory))
                {
                    throw new InvalidOperationException($"The template directory \"{CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory}\" does not exist");
                }
            }

            if (CodeGenerators.OpenApiToCSharpClientCommand != null)
            {
                CodeGenerators.OpenApiToCSharpClientCommand.ContractsOutputFilePath = ConvertToAbsolutePath(
                    CodeGenerators.OpenApiToCSharpClientCommand.ContractsOutputFilePath);
                CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory = ConvertToAbsolutePath(
                    CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory);

                if (CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory != null && !Directory.Exists(CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory))
                {
                    throw new InvalidOperationException($"The template directory \"{CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory}\" does not exist");
                }
            }

            if (CodeGenerators.OpenApiToCSharpControllerCommand != null)
            {
                CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory = ConvertToAbsolutePath(
                    CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory);

                if (CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory != null && !Directory.Exists(CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory))
                {
                    throw new InvalidOperationException($"The template directory {CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory}\" does not exist");
                }
            }

            foreach (var generator in CodeGenerators.Items.Concat(SwaggerGenerators.Items))
            {
                generator.OutputFilePath = ConvertToAbsolutePath(generator.OutputFilePath);
            }
        }

        private void ConvertToRelativePaths()
        {
            if (SwaggerGenerators.FromDocumentCommand != null)
            {
                if (!SwaggerGenerators.FromDocumentCommand.Url.StartsWith("http://") && !SwaggerGenerators.FromDocumentCommand.Url.StartsWith("https://"))
                {
                    SwaggerGenerators.FromDocumentCommand.Url = ConvertToRelativePath(SwaggerGenerators.FromDocumentCommand.Url);
                }
            }

            if (SwaggerGenerators.WebApiToOpenApiCommand != null)
            {
                SwaggerGenerators.WebApiToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.WebApiToOpenApiCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
                SwaggerGenerators.WebApiToOpenApiCommand.ReferencePaths =
                    SwaggerGenerators.WebApiToOpenApiCommand.ReferencePaths.Select(ConvertToRelativePath).ToArray();

                SwaggerGenerators.WebApiToOpenApiCommand.DocumentTemplate = ConvertToRelativePath(
                    SwaggerGenerators.WebApiToOpenApiCommand.DocumentTemplate);
                SwaggerGenerators.WebApiToOpenApiCommand.AssemblyConfig = ConvertToRelativePath(
                    SwaggerGenerators.WebApiToOpenApiCommand.AssemblyConfig);
            }

            if (SwaggerGenerators.AspNetCoreToOpenApiCommand != null)
            {
                SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
                SwaggerGenerators.AspNetCoreToOpenApiCommand.ReferencePaths =
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.ReferencePaths.Select(ConvertToRelativePath).ToArray();

                SwaggerGenerators.AspNetCoreToOpenApiCommand.DocumentTemplate = ConvertToRelativePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.DocumentTemplate);
                SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyConfig = ConvertToRelativePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.AssemblyConfig);

                SwaggerGenerators.AspNetCoreToOpenApiCommand.Project = ConvertToRelativePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.Project);
                SwaggerGenerators.AspNetCoreToOpenApiCommand.MSBuildProjectExtensionsPath = ConvertToRelativePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.MSBuildProjectExtensionsPath);

                SwaggerGenerators.AspNetCoreToOpenApiCommand.WorkingDirectory = ConvertToRelativePath(
                    SwaggerGenerators.AspNetCoreToOpenApiCommand.WorkingDirectory);
            }


            if (SwaggerGenerators.TypesToOpenApiCommand != null)
            {
                SwaggerGenerators.TypesToOpenApiCommand.AssemblyPaths =
                    SwaggerGenerators.TypesToOpenApiCommand.AssemblyPaths.Select(ConvertToRelativePath).ToArray();
                SwaggerGenerators.TypesToOpenApiCommand.AssemblyConfig = ConvertToRelativePath(
                    SwaggerGenerators.TypesToOpenApiCommand.AssemblyConfig);
            }

            if (CodeGenerators.OpenApiToTypeScriptClientCommand != null)
            {
                CodeGenerators.OpenApiToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(
                    CodeGenerators.OpenApiToTypeScriptClientCommand.ExtensionCode);
                CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory = ConvertToRelativePath(
                    CodeGenerators.OpenApiToTypeScriptClientCommand.TemplateDirectory);
            }

            if (CodeGenerators.OpenApiToCSharpClientCommand != null)
            {
                CodeGenerators.OpenApiToCSharpClientCommand.ContractsOutputFilePath = ConvertToRelativePath(
                    CodeGenerators.OpenApiToCSharpClientCommand.ContractsOutputFilePath);
                CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory = ConvertToRelativePath(
                    CodeGenerators.OpenApiToCSharpClientCommand.TemplateDirectory);
            }

            if (CodeGenerators.OpenApiToCSharpControllerCommand != null)
            {
                CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory = ConvertToRelativePath(
                    CodeGenerators.OpenApiToCSharpControllerCommand.TemplateDirectory);
            }

            foreach (var generator in CodeGenerators.Items.Where(i => i != null).Concat(SwaggerGenerators.Items))
            {
                generator.OutputFilePath = ConvertToRelativePath(generator.OutputFilePath);
            }
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

            // Swagger to OpenApi rename
            if (data.Contains("\"typeScriptVersion\":") && !data.ToLowerInvariant().Contains("ExceptionClass".ToLowerInvariant()))
            {
                data = data.Replace("\"typeScriptVersion\":", "\"exceptionClass\": \"SwaggerException\", \"typeScriptVersion\":");
                saveFile = true;
            }

            if (data.Contains("\"swaggerGenerator\":"))
            {
                data = data.Replace("\"swaggerGenerator\":", "\"documentGenerator\":");
                saveFile = true;
            }

            if (data.Contains("\"swaggerToTypeScriptClient\":"))
            {
                data = data.Replace("\"swaggerToTypeScriptClient\":", "\"openApiToTypeScriptClient\":");
                saveFile = true;
            }

            if (data.Contains("\"swaggerToCSharpClient\":"))
            {
                data = data.Replace("\"swaggerToCSharpClient\":", "\"openApiToCSharpClient\":");
                saveFile = true;
            }

            if (data.Contains("\"swaggerToCSharpController\":"))
            {
                data = data.Replace("\"swaggerToCSharpController\":", "\"openApiToCSharpController\":");
                saveFile = true;
            }

            if (data.Contains("\"fromSwagger\":"))
            {
                data = data.Replace("\"fromSwagger\":", "\"fromDocument\":");
                saveFile = true;
            }

            if (data.Contains("\"jsonSchemaToSwagger\":"))
            {
                data = data.Replace("\"jsonSchemaToSwagger\":", "\"jsonSchemaToOpenApi\":");
                saveFile = true;
            }

            if (data.Contains("\"webApiToSwagger\":"))
            {
                data = data.Replace("\"webApiToSwagger\":", "\"webApiToOpenApi\":");
                saveFile = true;
            }

            if (data.Contains("\"aspNetCoreToSwagger\":"))
            {
                data = data.Replace("\"aspNetCoreToSwagger\":", "\"aspNetCoreToOpenApi\":");
                saveFile = true;
            }

            if (data.Contains("\"typesToSwagger\":"))
            {
                data = data.Replace("\"typesToSwagger\":", "\"typesToOpenApi\":");
                saveFile = true;
            }

            // New file format
            if (data.Contains("\"aspNetNamespace\": \"System.Web.Http\""))
            {
                data = data.Replace("\"aspNetNamespace\": \"System.Web.Http\"", "\"controllerTarget\": \"AspNet\"");
                saveFile = true;
            }

            if (data.Contains("\"aspNetNamespace\": \"Microsoft.AspNetCore.Mvc\""))
            {
                data = data.Replace("\"aspNetNamespace\": \"Microsoft.AspNetCore.Mvc\"", "\"controllerTarget\": \"AspNetCore\"");
                saveFile = true;
            }

            if (data.Contains("\"noBuild\":") && !data.ToLowerInvariant().Contains("UseDocumentProvider".ToLowerInvariant()))
            {
                data = data.Replace("\"noBuild\":", "\"useDocumentProvider\": false, \"noBuild\":");
                saveFile = true;
            }

            if (data.Contains("\"noBuild\":") && !data.ToLowerInvariant().Contains("RequireParametersWithoutDefault".ToLowerInvariant()))
            {
                data = data.Replace("\"noBuild\":", "\"requireParametersWithoutDefault\": true, \"noBuild\":");
                saveFile = true;
            }

            if (data.Contains("assemblyTypeToSwagger"))
            {
                data = data.Replace("assemblyTypeToSwagger", "typesToSwagger");
                saveFile = true;
            }

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
