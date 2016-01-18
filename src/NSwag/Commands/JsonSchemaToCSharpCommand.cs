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

        [Description("The date time .NET type (default: 'DateTime').")]
        [Argument(Name = "DateTimeType", DefaultValue = "DateTime")]
        public string DateTimeType { get; set; }

        [Description("The generic array .NET type (default: 'ObservableCollection').")]
        [Argument(Name = "ArrayType", DefaultValue = "ObservableCollection")]
        public string ArrayType { get; set; }

        [Description("The generic dictionary .NET type (default: 'Dictionary').")]
        [Argument(Name = "DictionaryType", DefaultValue = "Dictionary")]
        public string DictionaryType { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var settings = new CSharpGeneratorSettings
            {
                Namespace = Namespace,
                RequiredPropertiesMustBeDefined = RequiredPropertiesMustBeDefined,
                DateTimeType = DateTimeType,
                ArrayType = ArrayType,
                DictionaryType = DictionaryType,
            };
            
            var schema = JsonSchema4.FromJson(InputJson);
            var generator = new CSharpGenerator(schema, settings);
            var code = generator.GenerateFile();
            WriteOutput(host, code);
        }
    }
}