//-----------------------------------------------------------------------
// <copyright file="JsonSchemaToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "jsonschema2csclient", Description = "Generates CSharp classes from a JSON Schema.")]
    public class JsonSchemaToCSharpCommand : InputOutputCommandBase
    {
        public JsonSchemaToCSharpCommand()
        {
            Settings = new CSharpGeneratorSettings();
        }

        [JsonIgnore]
        public CSharpGeneratorSettings Settings { get; set; }

        [Argument(Name = "Name", Description = "The class name of the root schema.")]
        public string Name { get; set; }

        [Argument(Name = "Namespace", Description = "The namespace of the generated classes.")]
        public string Namespace
        {
            get { return Settings.Namespace; }
            set { Settings.Namespace = value; }
        }

        [Argument(Name = "RequiredPropertiesMustBeDefined",
                  Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).",
                  IsRequired = false)]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.RequiredPropertiesMustBeDefined; }
            set { Settings.RequiredPropertiesMustBeDefined = value; }
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time .NET type (default: 'DateTime').")]
        public string DateTimeType
        {
            get { return Settings.DateTimeType; }
            set { Settings.DateTimeType = value; }
        }

        [Argument(Name = "ArrayType", IsRequired = false, Description = "The generic array .NET type (default: 'ObservableCollection').")]
        public string ArrayType
        {
            get { return Settings.ArrayType; }
            set { Settings.ArrayType = value; }
        }

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryType
        {
            get { return Settings.DictionaryType; }
            set { Settings.DictionaryType = value; }
        }

        public override Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var schema = JsonSchema4.FromJson(InputJson);
            var generator = new CSharpGenerator(schema, Settings);

            var code = generator.GenerateFile(Name); 
            if (TryWriteFileOutput(host, () => code) == false)
                return Task.FromResult<object>(code);

            return Task.FromResult<object>(null);
        }
    }
}