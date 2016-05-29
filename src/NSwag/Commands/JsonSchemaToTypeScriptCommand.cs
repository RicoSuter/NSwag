using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates TypeScript interfaces from a JSON Schema.")]
    public class JsonSchemaToTypeScriptCommand : InputOutputCommandBase
    {
        public override Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var schema = JsonSchema4.FromJson(InputJson);
            var generator = new TypeScriptGenerator(schema);
            var code = generator.GenerateFile();
            TryWriteFileOutput(host, () => code);
            return Task.FromResult<object>(code);
        }
    }
}