using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates CSharp classes from a JSON Schema.")]
    public class JsonSchemaToCSharpCommand : InputOutputCommandBase
    {
        [Description("The namespace of the generated classes.")]
        [Argument(Name = "Namespace")]
        public string Namespace { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var schema = JsonSchema4.FromJson(InputJson);
            var generator = new CSharpGenerator(schema);
            generator.Namespace = Namespace; 
            var code = generator.GenerateFile();
            WriteOutput(host, code);
        }
    }
}