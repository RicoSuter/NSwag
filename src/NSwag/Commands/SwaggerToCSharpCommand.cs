using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.ClientGenerators.CSharp;

namespace NSwag.Commands
{
    [Description("Generates the CSharp client code.")]
    public class SwaggerToCSharpCommand : InputOutputCommandBase
    {
        [Description("The class name of the generated client.")]
        [Argument(Name = "Class")]
        public string Class { get; set; }

        [Description("The namespace of the generated client class.")]
        [Argument(Name = "Namespace")]
        public string Namespace { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var clientGenerator = new SwaggerToCSharpGenerator(InputSwaggerService);
            clientGenerator.Class = Class;
            clientGenerator.Namespace = Namespace;

            var output = clientGenerator.GenerateFile();
            WriteOutput(host, output);
        }
    }
}