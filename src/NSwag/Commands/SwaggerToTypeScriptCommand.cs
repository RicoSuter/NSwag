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
    public class SwaggerToTypeScriptCommand : InputOutputCommandBase
    {
        public SwaggerToTypeScriptCommand()
        {
            Settings = new SwaggerToTypeScriptClientGeneratorSettings();
        }

        [JsonIgnore]
        public SwaggerToTypeScriptClientGeneratorSettings Settings { get; set; }

        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName", DefaultValue = "{controller}Client")]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Description("The TypeScript module name (default: '', no module).")]
        [Argument(Name = "ModuleName", DefaultValue = "")]
        public string ModuleName
        {
            get { return Settings.ModuleName; }
            set { Settings.ModuleName = value; }
        }

        [Description("The type of the asynchronism handling ('JQueryCallbacks', 'JQueryPromises', 'AngularJS').")]
        [Argument(Name = "Template", DefaultValue = TypeScriptTemplate.JQueryCallbacks)]
        public TypeScriptTemplate Template
        {
            get { return Settings.Template; }
            set { Settings.Template = value; }
        }

        [Description("The promise type ('Promise' or 'QPromise').")]
        [Argument(Name = "PromiseType", DefaultValue = "Promise")]
        public PromiseType PromiseType
        {
            get { return Settings.PromiseType; }
            set { Settings.PromiseType = value; }
        }

        [Description("Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", DefaultValue = true)]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Description("Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", DefaultValue = false)]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", DefaultValue = true)]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Description("Specifies whether to generate readonly keywords (only available in TS 2.0+, default: true).")]
        [Argument(Name = "GenerateReadOnlyKeywords", DefaultValue = true)]
        public bool GenerateReadOnlyKeywords
        {
            get { return Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords; }
            set { Settings.TypeScriptGeneratorSettings.GenerateReadOnlyKeywords = value; }
        }

        [Description("The type style (default: Interface).")]
        [Argument(Name = "TypeStyle", DefaultValue = TypeScriptTypeStyle.Interface)]
        public TypeScriptTypeStyle TypeStyle
        {
            get { return Settings.TypeScriptGeneratorSettings.TypeStyle; }
            set { Settings.TypeScriptGeneratorSettings.TypeStyle = value; }
        }

        // TODO: Implement a way to pass mappings via cmd line
        //[Description("Type class mappings.")]
        //[Argument(Name = "ClassMappings", DefaultValue = null)]
        public TypeScriptClassMapping[] ClassMappings
        {
            get { return Settings.TypeScriptGeneratorSettings.ClassMappings; }
            set { Settings.TypeScriptGeneratorSettings.ClassMappings = value; }
        }

        [Description("The additional code (string or file path).")]
        [Argument(Name = "AdditionalCode", DefaultValue = "")]
        public string AdditionalCode { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var output = await RunAsync();
            WriteOutput(host, output);
        }

        public async Task<string> RunAsync()
        {
            var additionalCode = AdditionalCode ?? "";
            if (File.Exists(additionalCode))
                additionalCode = File.ReadAllText(additionalCode);
            Settings.TypeScriptGeneratorSettings.AdditionalCode = additionalCode;

            var clientGenerator = new SwaggerToTypeScriptClientGenerator(InputSwaggerService, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}