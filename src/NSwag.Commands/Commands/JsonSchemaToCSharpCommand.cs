//-----------------------------------------------------------------------
// <copyright file="JsonSchemaToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "jsonschema2csclient", Description = "Generates CSharp classes from a JSON Schema.")]
    public class JsonSchemaToCSharpCommand : InputOutputCommandBase
    {
        [Argument(Name = "Namespace", Description = "The namespace of the generated classes.")]
        public string Namespace { get; set; }

        [Argument(Name = "RequiredPropertiesMustBeDefined", 
                  Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).", 
                  IsRequired = false)]
        public bool RequiredPropertiesMustBeDefined { get; set; } = true;

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time .NET type (default: 'DateTime').")]
        public string DateTimeType { get; set; } = "DateTime";

        [Argument(Name = "ArrayType", IsRequired = false, Description = "The generic array .NET type (default: 'ObservableCollection').")]
        public string ArrayType { get; set; } = "ObservableCollection";

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
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