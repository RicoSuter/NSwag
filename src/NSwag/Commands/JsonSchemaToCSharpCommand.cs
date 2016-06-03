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
        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false)]
        public bool RequiredPropertiesMustBeDefined { get; set; } = true;

        [Description("The date time .NET type (default: 'DateTime').")]
        [Argument(Name = "DateTimeType", IsRequired = false)]
        public string DateTimeType { get; set; } = "DateTime";

        [Description("The generic array .NET type (default: 'ObservableCollection').")]
        [Argument(Name = "ArrayType", IsRequired = false)]
        public string ArrayType { get; set; } = "ObservableCollection";

        [Description("The generic dictionary .NET type (default: 'Dictionary').")]
        [Argument(Name = "DictionaryType", IsRequired = false)]
        public string DictionaryType { get; set; } = "Dictionary";

        public override Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
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
            if (TryWriteFileOutput(host, () => code) == false)
                return Task.FromResult<object>(code);
            return Task.FromResult<object>(null);
        }
    }
}