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

        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false,
                  Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.RequiredPropertiesMustBeDefined; }
            set { Settings.RequiredPropertiesMustBeDefined = value; }
        }

        [Argument(Name = "DateType", IsRequired = false, Description = "The date .NET type (default: 'DateTimeOffset').")]
        public string DateType
        {
            get { return Settings.DateType; }
            set { Settings.DateType = value; }
        }

        [Argument(Name = "JsonConverters", IsRequired = false, Description = "Specifies the custom Json.NET converter types (optional, comma separated).")]
        public string[] JsonConverters
        {
            get { return Settings.JsonConverters; }
            set { Settings.JsonConverters = value; }
        }

        [Argument(Name = "AnyType", IsRequired = false, Description = "The any .NET type (default: 'object').")]
        public string AnyType
        {
            get { return Settings.AnyType; }
            set { Settings.AnyType = value; }
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time .NET type (default: 'DateTimeOffset').")]
        public string DateTimeType
        {
            get { return Settings.DateTimeType; }
            set { Settings.DateTimeType = value; }
        }

        [Argument(Name = "TimeType", IsRequired = false, Description = "The time .NET type (default: 'TimeSpan').")]
        public string TimeType
        {
            get { return Settings.TimeType; }
            set { Settings.TimeType = value; }
        }

        [Argument(Name = "TimeSpanType", IsRequired = false, Description = "The time span .NET type (default: 'TimeSpan').")]
        public string TimeSpanType
        {
            get { return Settings.TimeSpanType; }
            set { Settings.TimeSpanType = value; }
        }

        [Argument(Name = "ArrayType", IsRequired = false, Description = "The generic array .NET type (default: 'ICollection').")]
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

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'IDictionary').")]
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

        [Argument(Name = "ArrayBaseType", IsRequired = false, Description = "The generic array .NET type (default: 'Collection').")]
        public string ArrayBaseType
        {
            get { return Settings.ArrayBaseType; }
            set { Settings.ArrayBaseType = value; }
        }

        [Argument(Name = "DictionaryBaseType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryBaseType
        {
            get { return Settings.DictionaryBaseType; }
            set { Settings.DictionaryBaseType = value; }
        }

        [Argument(Name = "ClassStyle", IsRequired = false, Description = "The CSharp class style, 'Poco' or 'Inpc' (default: 'Poco').")]
        public CSharpClassStyle ClassStyle
        {
            get { return Settings.ClassStyle; }
            set { Settings.ClassStyle = value; }
        }

        [Argument(Name = "JsonLibrary", IsRequired = false, Description = "The CSharp JSON library, 'NewtonsoftJson' or 'SystemTextJson' (default: 'NewtonsoftJson', 'SystemTextJson' is experimental).")]
        public CSharpJsonLibrary JsonLibrary
        {
            get { return Settings.JsonLibrary; }
            set { Settings.JsonLibrary = value; }
        }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (may generate CSharp 6 code, default: true).")]
        public bool GenerateDefaultValues
        {
            get { return Settings.GenerateDefaultValues; }
            set { Settings.GenerateDefaultValues = value; }
        }

        [Argument(Name = "GenerateDataAnnotations", IsRequired = false, Description = "Specifies whether to generate data annotation attributes on DTO classes (default: true).")]
        public bool GenerateDataAnnotations
        {
            get { return Settings.GenerateDataAnnotations; }
            set { Settings.GenerateDataAnnotations = value; }
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded DTO type names (must be defined in an import or other namespace).")]
        public string[] ExcludedTypeNames
        {
            get { return Settings.ExcludedTypeNames; }
            set { Settings.ExcludedTypeNames = value; }
        }

        [Argument(Name = "HandleReferences", IsRequired = false, Description = "Use preserve references handling (All) in the JSON serializer (default: false).")]
        public bool HandleReferences
        {
            get { return Settings.HandleReferences; }
            set { Settings.HandleReferences = value; }
        }

        [Argument(Name = "GenerateImmutableArrayProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable array properties (default: false).")]
        public bool GenerateImmutableArrayProperties
        {
            get { return Settings.GenerateImmutableArrayProperties; }
            set { Settings.GenerateImmutableArrayProperties = value; }
        }

        [Argument(Name = "GenerateImmutableDictionaryProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable dictionary properties (default: false).")]
        public bool GenerateImmutableDictionaryProperties
        {
            get { return Settings.GenerateImmutableDictionaryProperties; }
            set { Settings.GenerateImmutableDictionaryProperties = value; }
        }

        [Argument(Name = "JsonSerializerSettingsTransformationMethod", IsRequired = false,
            Description = "The name of a static method which is called to transform the JsonSerializerSettings used in the generated ToJson()/FromJson() methods (default: none).")]
        public string JsonSerializerSettingsTransformationMethod
        {
            get { return Settings.JsonSerializerSettingsTransformationMethod; }
            set { Settings.JsonSerializerSettingsTransformationMethod = value; }
        }

        [Argument(Name = "InlineNamedArrays", Description = "Inline named arrays (default: false).", IsRequired = false)]
        public bool InlineNamedArrays
        {
            get { return Settings.InlineNamedArrays; }
            set { Settings.InlineNamedArrays = value; }
        }

        [Argument(Name = "InlineNamedDictionaries", Description = "Inline named dictionaries (default: false).", IsRequired = false)]
        public bool InlineNamedDictionaries
        {
            get { return Settings.InlineNamedDictionaries; }
            set { Settings.InlineNamedDictionaries = value; }
        }

        [Argument(Name = "InlineNamedTuples", Description = "Inline named tuples (default: true).", IsRequired = false)]
        public bool InlineNamedTuples
        {
            get { return Settings.InlineNamedTuples; }
            set { Settings.InlineNamedTuples = value; }
        }

        [Argument(Name = "InlineNamedAny", Description = "Inline named any types (default: false).", IsRequired = false)]
        public bool InlineNamedAny
        {
            get { return Settings.InlineNamedAny; }
            set { Settings.InlineNamedAny = value; }
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