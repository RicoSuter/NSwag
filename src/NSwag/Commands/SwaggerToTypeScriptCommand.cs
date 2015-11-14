using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates TypeScript client code from a Swagger specification.")]
    public class SwaggerToTypeScriptCommand : InputOutputCommandBase
    {
        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName", DefaultValue = "{controller}Client")]
        public string ClassName { get; set; }

        [Description("The type of the asynchronism handling ('Callbacks' or 'Q').")]
        [Argument(Name = "AsyncType", DefaultValue = TypeScriptAsyncType.Callbacks)]
        public TypeScriptAsyncType AsyncType { get; set; }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", DefaultValue = true)]
        public bool GenerateDtoTypes { get; set; }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode { get; set; }
        
        [Description("Specifies whether to generate client interfaces.")]
        [Argument(Name = "GenerateClientInterfaces", DefaultValue = false)]
        public bool GenerateClientInterfaces { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var clientGenerator = new SwaggerToTypeScriptGenerator(InputSwaggerService, new SwaggerToTypeScriptGeneratorSettings
            {
                Class = ClassName,
                AsyncType = AsyncType,
                OperationGenerationMode = OperationGenerationMode,
                GenerateClientInterfaces = GenerateClientInterfaces,
                GenerateDtoTypes = GenerateDtoTypes
            });

            var output = clientGenerator.GenerateFile();
            WriteOutput(host, output);
        }
    }
}