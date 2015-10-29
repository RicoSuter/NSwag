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
        [Argument(Name = "Class", DefaultValue = "{controller}Client")]
        public string Class { get; set; }

        [Description("The type of the asynchronism handling ('Callbacks' or 'Q').")]
        [Argument(Name = "AsyncType", DefaultValue = TypeScriptAsyncType.Callbacks)]
        public TypeScriptAsyncType AsyncType { get; set; }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var clientGenerator = new SwaggerToTypeScriptGenerator(InputSwaggerService);
            clientGenerator.Class = Class;
            clientGenerator.AsyncType = AsyncType;
            clientGenerator.OperationGenerationMode = OperationGenerationMode;

            var output = clientGenerator.GenerateFile();
            WriteOutput(host, output);
        }
    }
}