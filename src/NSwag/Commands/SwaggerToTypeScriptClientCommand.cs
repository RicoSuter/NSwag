using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates TypeScript client code from a Swagger specification.")]
    public class SwaggerToTypeScriptClientCommand : InputOutputCommandBase
    {
        public SwaggerToTypeScriptClientCommand()
        {
            Settings = new SwaggerToTypeScriptClientGeneratorSettings();
        }

        [JsonIgnore]
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName", IsRequired = false)]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Description("The TypeScript module name (default: '', no module).")]
        [Argument(Name = "ModuleName", IsRequired = false)]
        public string ModuleName
        {
            get { return Settings.TypeScriptGeneratorSettings.ModuleName; }
            set { Settings.TypeScriptGeneratorSettings.ModuleName = value; }
        }

        [Description("The type of the asynchronism handling ('JQueryCallbacks', 'JQueryPromises', 'AngularJS', 'Angular2').")]
        [Argument(Name = "Template", IsRequired = false)]
        public TypeScriptTemplate Template
        {
            get { return Settings.Template; }
            set { Settings.Template = value; }
        }

        [Description("The promise type ('Promise' or 'QPromise').")]
        [Argument(Name = "PromiseType", IsRequired = false)]
        public PromiseType PromiseType
        {
            get { return Settings.PromiseType; }
            set { Settings.PromiseType = value; }
        }

        [Description("Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", IsRequired = false)]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Description("Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", IsRequired = false)]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", IsRequired = false)]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", IsRequired = false)]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Description("Specifies whether to generate readonly keywords (only available in TS 2.0+, default: true).")]
        [Argument(Name = "GenerateReadOnlyKeywords", IsRequired = false)]
        public bool GenerateReadOnlyKeywords
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords; }
            set { Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords = value; }
        }

        [Description("The type style (default: Interface).")]
        [Argument(Name = "TypeStyle", IsRequired = false)]
        public TypeScriptTypeStyle TypeStyle
        {
            get { return Settings.TypeScriptGeneratorSettings.TypeStyle; }
            set { Settings.TypeScriptGeneratorSettings.TypeStyle = value; }
        }

        [Description("The type names which always generate plain TypeScript classes.")]
        [Argument(Name = "ClassTypes", IsRequired = false)]
        public string[] ClassTypes
        {
            get { return Settings.TypeScriptGeneratorSettings.ClassTypes; }
            set { Settings.TypeScriptGeneratorSettings.ClassTypes = value; }
        }

        [Description("The list of extended classes.")]
        [Argument(Name = "ExtendedClasses", IsRequired = false)]
        public string[] ExtendedClasses
        {
            get { return Settings.TypeScriptGeneratorSettings.ExtendedClasses; }
            set { Settings.TypeScriptGeneratorSettings.ExtendedClasses = value; }
        }

        [Description("The extension code (string or file path).")]
        [Argument(Name = "ExtensionCode", IsRequired = false)]
        public string ExtensionCode { get; set; }

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
                if (File.Exists(additionalCode))
                    additionalCode = File.ReadAllText(additionalCode);
                Settings.TypeScriptGeneratorSettings.ExtensionCode = additionalCode;

                var clientGenerator = new SwaggerToTypeScriptClientGenerator(InputSwaggerService, Settings);
                return clientGenerator.GenerateFile();
            });
        }
    }
}