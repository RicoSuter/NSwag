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

        [Description("Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        [Argument(Name = "RequiredPropertiesMustBeDefined", DefaultValue = true)]
        public bool RequiredPropertiesMustBeDefined { get; set; }

        [Description("The date time .NET type (DateTime or DateTimeOffset).")]
        [Argument(Name = "DateTimeType", DefaultValue = CSharpDateTimeType.DateTime)]
        public CSharpDateTimeType DateTimeType { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var settings = new CSharpGeneratorSettings
            {
                Namespace = Namespace,
                RequiredPropertiesMustBeDefined = RequiredPropertiesMustBeDefined,
                DateTimeType = DateTimeType
            };
            
            var schema = JsonSchema4.FromJson(InputJson);
            var generator = new CSharpGenerator(schema, settings);
            var code = generator.GenerateFile();
            WriteOutput(host, code);
        }
    }
}