//-----------------------------------------------------------------------
// <copyright file="JsonSchemaToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
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

        [Argument(Name = "AnyType", IsRequired = false, Description = "The any .NET type (default: 'object').")]
        public string AnyType
        {
            get { return Settings.AnyType; }
            set { Settings.AnyType = value; }
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

        [Argument(Name = "ArrayInstanceType", IsRequired = false, Description = "The generic array .NET instance type (default: empty = ArrayType).")]
        public string ArrayInstanceType
        {
            get { return Settings.ArrayInstanceType; }
            set { Settings.ArrayInstanceType = value; }
        }

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryType
        {
            get { return Settings.DictionaryType; }
            set { Settings.DictionaryType = value; }
        }

        [Argument(Name = "DictionaryInstanceType", IsRequired = false, Description = "The generic dictionary .NET instance type (default: empty = DictionaryType).")]
        public string DictionaryInstanceType
        {
            get { return Settings.DictionaryInstanceType; }
            set { Settings.DictionaryInstanceType = value; }
        }

        [Argument(Name = "GenerateOptionalPropertiesAsNullable", IsRequired = false, Description = "Specifies whether optional schema properties " +
            "(not required) are generated as nullable properties (default: false).")]
        public bool GenerateOptionalPropertiesAsNullable
        {
            get { return Settings.GenerateOptionalPropertiesAsNullable; }
            set { Settings.GenerateOptionalPropertiesAsNullable = value; }
        }

        [Argument(Name = "GenerateNullableReferenceTypes", IsRequired = false, Description = "Specifies whether whether to " +
            "generate Nullable Reference Type annotations (default: false).")]
        public bool GenerateNullableReferenceTypes
        {
            get { return Settings.GenerateNullableReferenceTypes; }
            set { Settings.GenerateNullableReferenceTypes = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var schema = await GetJsonSchemaAsync().ConfigureAwait(false);
            var generator = new CSharpGenerator(schema, Settings);

            var code = generator.GenerateFile(Name);
            await TryWriteFileOutputAsync(host, () => code).ConfigureAwait(false);
            return code;
        }
    }
}