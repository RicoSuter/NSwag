using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpCommand : InputOutputCommandBase
    {
        [Description("The class name of the generated client.")]
        [Argument(Name = "Class")]
        public string Class { get; set; }

        [Description("The namespace of the generated classes.")]
        [Argument(Name = "Namespace")]
        public string Namespace { get; set; }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var clientGenerator = new SwaggerToCSharpGenerator(InputSwaggerService);
            clientGenerator.Class = Class;
            clientGenerator.Namespace = Namespace;
            clientGenerator.OperationGenerationMode = OperationGenerationMode;

            var output = clientGenerator.GenerateFile();
            WriteOutput(host, output);
        }
    }
}