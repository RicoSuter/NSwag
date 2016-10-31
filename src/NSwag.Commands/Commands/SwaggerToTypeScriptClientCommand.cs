//-----------------------------------------------------------------------
// <copyright file="SwaggerToTypeScriptClientCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "swagger2tsclient", Description = "Generates TypeScript client code from a Swagger specification.")]
    public class SwaggerToTypeScriptClientCommand : InputOutputCommandBase
    {
        public SwaggerToTypeScriptClientCommand()
        {
            Settings = new SwaggerToTypeScriptClientGeneratorSettings();
        }

        [JsonIgnore]
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        [Argument(Name = "ClassName", IsRequired = false, Description = "The class name of the generated client.")]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Argument(Name = "ModuleName", IsRequired = false, Description = "The TypeScript module name (default: '', no module).")]
        public string ModuleName
        {
            get { return Settings.TypeScriptGeneratorSettings.ModuleName; }
            set { Settings.TypeScriptGeneratorSettings.ModuleName = value; }
        }

        [Argument(Name = "Namespace", IsRequired = false, Description = "The TypeScript namespace (default: '', no namespace).")]
        public string Namespace
        {
            get { return Settings.TypeScriptGeneratorSettings.Namespace; }
            set { Settings.TypeScriptGeneratorSettings.Namespace = value; }
        }

        [Argument(Name = "Template", IsRequired = false, Description = "The type of the asynchronism handling ('JQueryCallbacks', 'JQueryPromises', 'AngularJS', 'Angular2').")]
        public TypeScriptTemplate Template
        {
            get { return Settings.Template; }
            set { Settings.Template = value; }
        }

        [Argument(Name = "PromiseType", IsRequired = false, Description = "The promise type ('Promise' or 'QPromise').")]
        public PromiseType PromiseType
        {
            get { return Settings.PromiseType; }
            set { Settings.PromiseType = value; }
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time type ('Date', 'MomentJS', 'string').")]
        public TypeScriptDateTimeType DateTimeType
        {
            get { return Settings.TypeScriptGeneratorSettings.DateTimeType; }
            set { Settings.TypeScriptGeneratorSettings.DateTimeType = value; }
        }

        [Argument(Name = "GenerateClientClasses", IsRequired = false, Description = "Specifies whether generate client classes.")]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Argument(Name = "GenerateClientInterfaces", IsRequired = false, Description = "Specifies whether generate interfaces for the client classes.")]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Argument(Name = "GenerateDtoTypes", IsRequired = false, Description = "Specifies whether to generate DTO classes.")]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Argument(Name = "OperationGenerationMode", IsRequired = false, Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Argument(Name = "GenerateReadOnlyKeywords", IsRequired = false, Description = "Specifies whether to generate readonly keywords (only available in TS 2.0+, default: true).")]
        public bool GenerateReadOnlyKeywords
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords; }
            set { Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords = value; }
        }

        [Argument(Name = "TypeStyle", IsRequired = false, Description = "The type style (default: Class).")]
        public TypeScriptTypeStyle TypeStyle
        {
            get { return Settings.TypeScriptGeneratorSettings.TypeStyle; }
            set { Settings.TypeScriptGeneratorSettings.TypeStyle = value; }
        }

        [Argument(Name = "ClassTypes", IsRequired = false, Description = "The type names which always generate plain TypeScript classes.")]
        public string[] ClassTypes
        {
            get { return Settings.TypeScriptGeneratorSettings.ClassTypes; }
            set { Settings.TypeScriptGeneratorSettings.ClassTypes = value; }
        }

        [Argument(Name = "ExtendedClasses", IsRequired = false, Description = "The list of extended classes.")]
        public string[] ExtendedClasses
        {
            get { return Settings.TypeScriptGeneratorSettings.ExtendedClasses; }
            set { Settings.TypeScriptGeneratorSettings.ExtendedClasses = value; }
        }

        [Argument(Name = "ExtensionCode", IsRequired = false, Description = "The extension code (string or file path).")]
        public string ExtensionCode { get; set; }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (default: true).")]
        public bool GenerateDefaultValues
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateDefaultValues; }
            set { Settings.TypeScriptGeneratorSettings.GenerateDefaultValues = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = await RunAsync();
            if (TryWriteFileOutput(host, () => code) == false)
                return code;
            return null; 
        }

        public async Task<string> RunAsync()
        {
            return await Task.Run(() =>
            {
                var additionalCode = ExtensionCode ?? string.Empty;
                if (DynamicApis.FileExists(additionalCode))
                    additionalCode = DynamicApis.FileReadAllText(additionalCode);
                Settings.TypeScriptGeneratorSettings.ExtensionCode = additionalCode;

                var clientGenerator = new SwaggerToTypeScriptClientGenerator(InputSwaggerService, Settings);
                return clientGenerator.GenerateFile();
            });
        }
    }
}