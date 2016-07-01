//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyToolkit.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag.Commands;
using NSwag.Utilities;

namespace NSwag
{
    public class NSwagDocument : ObservableObject
    {
        private string _path;

        private int _selectedSwaggerGenerator;
        private int _selectedClientGenerator;

        private string _latestData;

        public NSwagDocument()
        {
            WebApiToSwaggerCommand = new WebApiToSwaggerCommand();
            AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand();

            SwaggerToTypeScriptClientCommand = new SwaggerToTypeScriptClientCommand();
            SwaggerToCSharpClientCommand = new SwaggerToCSharpClientCommand();
            SwaggerToCSharpControllerCommand = new SwaggerToCSharpControllerCommand();
        }

        public static NSwagDocument LoadDocument(string filePath)
        {
            var data = File.ReadAllText(filePath);

            var document = JsonConvert.DeserializeObject<NSwagDocument>(data);
            document.Path = filePath;
            document.ConvertToAbsolutePaths();

            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());

            // Legacy file support
            if (document.SwaggerToCSharpClientCommand.DateTimeType == "0")
                document.SwaggerToCSharpClientCommand.DateTimeType = "DateTime";
            document.WebApiToSwaggerCommand.ControllerName = "";

            return document;
        }

        public void Save()
        {
            ConvertToRelativePaths();

            _latestData = JsonConvert.SerializeObject(this, Formatting.Indented, GetSerializerSettings());
            ConvertToAbsolutePaths();
            File.WriteAllText(Path, _latestData);
        }

        private void ConvertToAbsolutePaths()
        {
            WebApiToSwaggerCommand.AssemblyPath = ConvertToAbsolute(WebApiToSwaggerCommand.AssemblyPath);
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToAbsolute(AssemblyTypeToSwaggerCommand.AssemblyPath);
            WebApiToSwaggerCommand.AssemblyConfig = ConvertToAbsolute(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToAbsolute(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToAbsolute(SwaggerToTypeScriptClientCommand.ExtensionCode);

            AssemblyTypeToSwaggerCommand.OutputFilePath = ConvertToAbsolute(AssemblyTypeToSwaggerCommand.OutputFilePath);
            SwaggerToCSharpClientCommand.OutputFilePath = ConvertToAbsolute(SwaggerToCSharpClientCommand.OutputFilePath);
            SwaggerToCSharpControllerCommand.OutputFilePath = ConvertToAbsolute(SwaggerToCSharpControllerCommand.OutputFilePath);
            SwaggerToTypeScriptClientCommand.OutputFilePath = ConvertToAbsolute(SwaggerToTypeScriptClientCommand.OutputFilePath);
            WebApiToSwaggerCommand.OutputFilePath = ConvertToAbsolute(WebApiToSwaggerCommand.OutputFilePath);
        }

        private string ConvertToAbsolute(string path)
        {
            if (!string.IsNullOrEmpty(path) && !System.IO.Path.IsPathRooted(path))
                return PathUtilities.MakeAbsolutePath(System.IO.Path.GetDirectoryName(Path), path);
            return path;
        }

        private void ConvertToRelativePaths()
        {
            WebApiToSwaggerCommand.AssemblyPath = ConvertToRelativePath(WebApiToSwaggerCommand.AssemblyPath);
            AssemblyTypeToSwaggerCommand.AssemblyPath = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyPath);
            WebApiToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(WebApiToSwaggerCommand.AssemblyConfig);
            AssemblyTypeToSwaggerCommand.AssemblyConfig = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.AssemblyConfig);

            SwaggerToTypeScriptClientCommand.ExtensionCode = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.ExtensionCode);

            AssemblyTypeToSwaggerCommand.OutputFilePath = ConvertToRelativePath(AssemblyTypeToSwaggerCommand.OutputFilePath);
            SwaggerToCSharpClientCommand.OutputFilePath = ConvertToRelativePath(SwaggerToCSharpClientCommand.OutputFilePath);
            SwaggerToCSharpControllerCommand.OutputFilePath = ConvertToRelativePath(SwaggerToCSharpControllerCommand.OutputFilePath);
            SwaggerToTypeScriptClientCommand.OutputFilePath = ConvertToRelativePath(SwaggerToTypeScriptClientCommand.OutputFilePath);
            WebApiToSwaggerCommand.OutputFilePath = ConvertToRelativePath(WebApiToSwaggerCommand.OutputFilePath);
        }

        private string ConvertToRelativePath(string path)
        {
            if (!string.IsNullOrEmpty(path))
                return PathUtilities.MakeRelativePath(path, System.IO.Path.GetDirectoryName(Path));
            return path;
        }

        public static NSwagDocument CreateDocument()
        {
            var document = new NSwagDocument();
            document.Path = "Untitled";
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented, GetSerializerSettings());
            return document;
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
            // TODO: Improve whole document class!

            SwaggerService service = null; 

            if (!string.IsNullOrEmpty(SwaggerToCSharpClientCommand.OutputFilePath))
            {
                if (service == null)
                    service = await GenerateService();

                SwaggerToCSharpClientCommand.Input = service;
                await SwaggerToCSharpClientCommand.RunAsync(null, null);
                SwaggerToCSharpClientCommand.Input = null;
            }

            if (!string.IsNullOrEmpty(SwaggerToCSharpControllerCommand.OutputFilePath))
            {
                if (service == null)
                    service = await GenerateService();

                SwaggerToCSharpControllerCommand.Input = service;
                await SwaggerToCSharpControllerCommand.RunAsync(null, null);
                SwaggerToCSharpControllerCommand.Input = null;
            }

            if (!string.IsNullOrEmpty(SwaggerToTypeScriptClientCommand.OutputFilePath))
            {
                if (service == null)
                    service = await GenerateService();

                SwaggerToTypeScriptClientCommand.Input = service; 
                await SwaggerToTypeScriptClientCommand.RunAsync(null, null);
                SwaggerToTypeScriptClientCommand.Input = null;
            }
        }

        private async Task<SwaggerService> GenerateService()
        {
            if (SelectedSwaggerGenerator == 0)
                return SwaggerService.FromJson(InputSwagger);
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
    }
}
