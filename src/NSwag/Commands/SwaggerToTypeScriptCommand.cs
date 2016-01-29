using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
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

        [Description("The type of the asynchronism handling ('JQueryCallbacks', 'JQueryQPromises', 'AngularJS').")]
        [Argument(Name = "Template", DefaultValue = TypeScriptTemplate.JQueryCallbacks)]
        public TypeScriptTemplate Template
        {
            get { return Settings.Template; }
            set { Settings.Template = value; }
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

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var output = await RunAsync();
            WriteOutput(host, output);
        }

        public async Task<string> RunAsync()
        {
            var clientGenerator = new SwaggerToTypeScriptClientGenerator(InputSwaggerService, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}