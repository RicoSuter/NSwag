//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using MyToolkit.Model;
using Newtonsoft.Json;
using NSwag.Commands;

namespace NSwagStudio
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

            SwaggerToTypeScriptCommand = new SwaggerToTypeScriptCommand();
            SwaggerToCSharpCommand = new SwaggerToCSharpCommand();
        }

        public static NSwagDocument LoadDocument(string filePath)
        {
            var data = System.IO.File.ReadAllText(filePath);
            var document = JsonConvert.DeserializeObject<NSwagDocument>(data);
            document.Path = filePath;
            document._latestData = data;

            // Legacy file support
            if (document.SwaggerToCSharpCommand.DateTimeType == "0")
                document.SwaggerToCSharpCommand.DateTimeType = "DateTime";

            return document;
        }

        public static NSwagDocument CreateDocument()
        {
            var document = new NSwagDocument();
            document.Path = "Untitled";
            document._latestData = JsonConvert.SerializeObject(document, Formatting.Indented);
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
        public string Name
        {
            get { return System.IO.Path.GetFileName(Path); }
        }

        [JsonIgnore]
        public bool IsDirty
        {
            get { return _latestData != JsonConvert.SerializeObject(this, Formatting.Indented); }
        }

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

        [JsonProperty("InputJsonSchema")]
        public string InputJsonSchema { get; set; }

        [JsonProperty("WebApiToSwaggerCommand")]
        public WebApiToSwaggerCommand WebApiToSwaggerCommand { get; set; }

        [JsonProperty("AssemblyTypeToSwaggerCommand")]
        public AssemblyTypeToSwaggerCommand AssemblyTypeToSwaggerCommand { get; set; }

        [JsonProperty("SwaggerToTypeScriptCommand")]
        public SwaggerToTypeScriptCommand SwaggerToTypeScriptCommand { get; set; }

        [JsonProperty("SwaggerToCSharpCommand")]
        public SwaggerToCSharpCommand SwaggerToCSharpCommand { get; set; }

        public void Save()
        {
            _latestData = JsonConvert.SerializeObject(this, Formatting.Indented); 
            System.IO.File.WriteAllText(Path, _latestData);
        }
    }
}
