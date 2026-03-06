//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
{
    public abstract class OpenApiToCSharpCommandBase<TSettings> : CodeGeneratorCommandBase<TSettings>
         where TSettings : CSharpGeneratorBaseSettings
    {
        protected OpenApiToCSharpCommandBase(TSettings settings)
            : base(settings)
        {
        }

        [Argument(Name = "ClassName", IsRequired = false, Description = "The class name of the generated client.")]
        public string ClassName
        {
            get => Settings.ClassName;
            set => Settings.ClassName = value;
        }

        [Argument(Name = "OperationGenerationMode", IsRequired = false, Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        public OperationGenerationMode OperationGenerationMode
        {
            get => OperationGenerationModeConverter.GetOperationGenerationMode(Settings.OperationNameGenerator);
            set => Settings.OperationNameGenerator = OperationGenerationModeConverter.GetOperationNameGenerator(value);
        }

        [Argument(Name = "IncludedOperationIds", IsRequired = false, Description = "The operations that should be included or excluded.")]
        public string[] IncludedOperationIds
        {
            get => Settings.IncludedOperationIds;
            set => Settings.IncludedOperationIds = value;
        }

        [Argument(Name = "ExcludedOperationIds", IsRequired = false, Description = "The operations that should be included or excluded.")]
        public string[] ExcludedOperationIds
        {
            get => Settings.ExcludedOperationIds;
            set => Settings.ExcludedOperationIds = value;
        }

        [Argument(Name = "ExcludeDeprecated", IsRequired = false, Description = "Specifies if deprecated endpoints should be generated")]
        public bool ExcludeDeprecated
        {
            get => Settings.ExcludeDeprecated;
            set => Settings.ExcludeDeprecated = value;
        }

        [Argument(Name = "AdditionalNamespaceUsages", IsRequired = false, Description = "The additional namespace usages.")]
        public string[] AdditionalNamespaceUsages
        {
            get => Settings.AdditionalNamespaceUsages;
            set => Settings.AdditionalNamespaceUsages = value;
        }

        [Argument(Name = "AdditionalContractNamespaceUsages", IsRequired = false, Description = "The additional contract namespace usages.")]
        public string[] AdditionalContractNamespaceUsages
        {
            get => Settings.AdditionalContractNamespaceUsages;
            set => Settings.AdditionalContractNamespaceUsages = value;
        }

        [Argument(Name = "GenerateOptionalParameters", IsRequired = false,
                  Description = "Specifies whether to reorder parameters (required first, optional at the end) and generate optional parameters (default: false).")]
        public bool GenerateOptionalParameters
        {
            get => Settings.GenerateOptionalParameters;
            set => Settings.GenerateOptionalParameters = value;
        }

        [Argument(Name = "GenerateJsonMethods", IsRequired = false,
            Description = "Specifies whether to render ToJson() and FromJson() methods for DTOs (default: true).")]
        public bool GenerateJsonMethods
        {
            get => Settings.CSharpGeneratorSettings.GenerateJsonMethods;
            set => Settings.CSharpGeneratorSettings.GenerateJsonMethods = value;
        }

        [Argument(Name = "EnforceFlagEnums", IsRequired = false,
            Description = "Specifies whether enums should be always generated as bit flags (default: false).")]
        public bool EnforceFlagEnums
        {
            get => Settings.CSharpGeneratorSettings.EnforceFlagEnums;
            set => Settings.CSharpGeneratorSettings.EnforceFlagEnums = value;
        }

        [Argument(Name = "ParameterArrayType", IsRequired = false, Description = "The generic array .NET type of operation parameters (default: 'IEnumerable').")]
        public string ParameterArrayType
        {
            get => Settings.ParameterArrayType;
            set => Settings.ParameterArrayType = value;
        }

        [Argument(Name = "ParameterDictionaryType", IsRequired = false, Description = "The generic dictionary .NET type of operation parameters (default: 'IDictionary').")]
        public string ParameterDictionaryType
        {
            get => Settings.ParameterDictionaryType;
            set => Settings.ParameterDictionaryType = value;
        }

        [Argument(Name = "ResponseArrayType", IsRequired = false, Description = "The generic array .NET type of operation responses (default: 'ICollection').")]
        public string ResponseArrayType
        {
            get => Settings.ResponseArrayType;
            set => Settings.ResponseArrayType = value;
        }

        [Argument(Name = "ResponseDictionaryType", IsRequired = false, Description = "The generic dictionary .NET type of operation responses (default: 'IDictionary').")]
        public string ResponseDictionaryType
        {
            get => Settings.ResponseDictionaryType;
            set => Settings.ResponseDictionaryType = value;
        }

        [Argument(Name = "WrapResponses", IsRequired = false, Description = "Specifies whether to wrap success responses to allow full response access.")]
        public bool WrapResponses
        {
            get => Settings.WrapResponses;
            set => Settings.WrapResponses = value;
        }

        [Argument(Name = "WrapResponseMethods", IsRequired = false, Description = "List of methods where responses are wrapped ('ControllerName.MethodName', WrapResponses must be true).")]
        public string[] WrapResponseMethods
        {
            get => Settings.WrapResponseMethods;
            set => Settings.WrapResponseMethods = value;
        }

        [Argument(Name = "GenerateResponseClasses", IsRequired = false, Description = "Specifies whether to generate response classes (default: true).")]
        public bool GenerateResponseClasses
        {
            get => Settings.GenerateResponseClasses;
            set => Settings.GenerateResponseClasses = value;
        }

        [Argument(Name = "ResponseClass", IsRequired = false, Description = "The response class (default 'SwaggerResponse', may use '{controller}' placeholder).")]
        public string ResponseClass
        {
            get => Settings.ResponseClass;
            set => Settings.ResponseClass = value;
        }

        // CSharpGeneratorSettings

        [Argument(Name = "Namespace", Description = "The namespace of the generated classes.")]
        public string Namespace
        {
            get => Settings.CSharpGeneratorSettings.Namespace;
            set => Settings.CSharpGeneratorSettings.Namespace = value;
        }

        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false,
                  Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        public bool RequiredPropertiesMustBeDefined
        {
            get => Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined;
            set => Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = value;
        }

        [Argument(Name = "DateType", IsRequired = false, Description = "The date .NET type (default: 'DateTimeOffset').")]
        public string DateType
        {
            get => Settings.CSharpGeneratorSettings.DateType;
            set => Settings.CSharpGeneratorSettings.DateType = value;
        }

        [Argument(Name = "JsonConverters", IsRequired = false, Description = "Specifies the custom Json.NET converter types (optional, comma separated).")]
        public string[] JsonConverters
        {
            get => Settings.CSharpGeneratorSettings.JsonConverters;
            set => Settings.CSharpGeneratorSettings.JsonConverters = value;
        }

        [Argument(Name = "AnyType", IsRequired = false, Description = "The any .NET type (default: 'object').")]
        public string AnyType
        {
            get => Settings.CSharpGeneratorSettings.AnyType;
            set => Settings.CSharpGeneratorSettings.AnyType = value;
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time .NET type (default: 'DateTimeOffset').")]
        public string DateTimeType
        {
            get => Settings.CSharpGeneratorSettings.DateTimeType;
            set => Settings.CSharpGeneratorSettings.DateTimeType = value;
        }

        [Argument(Name = "TimeType", IsRequired = false, Description = "The time .NET type (default: 'TimeSpan').")]
        public string TimeType
        {
            get => Settings.CSharpGeneratorSettings.TimeType;
            set => Settings.CSharpGeneratorSettings.TimeType = value;
        }

        [Argument(Name = "TimeSpanType", IsRequired = false, Description = "The time span .NET type (default: 'TimeSpan').")]
        public string TimeSpanType
        {
            get => Settings.CSharpGeneratorSettings.TimeSpanType;
            set => Settings.CSharpGeneratorSettings.TimeSpanType = value;
        }

        [Argument(Name = "ArrayType", IsRequired = false, Description = "The generic array .NET type (default: 'ICollection').")]
        public string ArrayType
        {
            get => Settings.CSharpGeneratorSettings.ArrayType;
            set => Settings.CSharpGeneratorSettings.ArrayType = value;
        }

        [Argument(Name = "ArrayInstanceType", IsRequired = false, Description = "The generic array .NET instance type (default: empty = ArrayType).")]
        public string ArrayInstanceType
        {
            get => Settings.CSharpGeneratorSettings.ArrayInstanceType;
            set => Settings.CSharpGeneratorSettings.ArrayInstanceType = value;
        }

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'IDictionary').")]
        public string DictionaryType
        {
            get => Settings.CSharpGeneratorSettings.DictionaryType;
            set => Settings.CSharpGeneratorSettings.DictionaryType = value;
        }

        [Argument(Name = "DictionaryInstanceType", IsRequired = false, Description = "The generic dictionary .NET instance type (default: empty = DictionaryType).")]
        public string DictionaryInstanceType
        {
            get => Settings.CSharpGeneratorSettings.DictionaryInstanceType;
            set => Settings.CSharpGeneratorSettings.DictionaryInstanceType = value;
        }

        [Argument(Name = "ArrayBaseType", IsRequired = false, Description = "The generic array .NET type (default: 'Collection').")]
        public string ArrayBaseType
        {
            get => Settings.CSharpGeneratorSettings.ArrayBaseType;
            set => Settings.CSharpGeneratorSettings.ArrayBaseType = value;
        }

        [Argument(Name = "DictionaryBaseType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryBaseType
        {
            get => Settings.CSharpGeneratorSettings.DictionaryBaseType;
            set => Settings.CSharpGeneratorSettings.DictionaryBaseType = value;
        }

        [Argument(Name = "ClassStyle", IsRequired = false, Description = "The CSharp class style, 'Poco' or 'Inpc' (default: 'Poco').")]
        public CSharpClassStyle ClassStyle
        {
            get => Settings.CSharpGeneratorSettings.ClassStyle;
            set => Settings.CSharpGeneratorSettings.ClassStyle = value;
        }

        [Argument(Name = "JsonLibrary", IsRequired = false, Description = "The CSharp JSON library, 'NewtonsoftJson' or 'SystemTextJson' (default: 'NewtonsoftJson', 'SystemTextJson' is experimental).")]
        public CSharpJsonLibrary JsonLibrary
        {
            get => Settings.CSharpGeneratorSettings.JsonLibrary;
            set => Settings.CSharpGeneratorSettings.JsonLibrary = value;
        }

        [Argument(Name = "JsonPolymorphicSerializationStyle", IsRequired = false, Description = "The CSharp JSON polymorphic serialization style, 'NJsonSchema' or 'SystemTextJson' (default: 'NJsonSchema', 'SystemTextJson' is experimental).")]
        public CSharpJsonPolymorphicSerializationStyle JsonPolymorphicSerializationStyle
        {
            get => Settings.CSharpGeneratorSettings.JsonPolymorphicSerializationStyle;
            set => Settings.CSharpGeneratorSettings.JsonPolymorphicSerializationStyle = value;
        }

        [Argument(Name = "JsonLibraryVersion", IsRequired = false, Description = "The CSharp JSON library version to use (applies only to System.Text.Json, default: 8.0).")]
        public decimal JsonLibraryVersion
        {
            get => Settings.CSharpGeneratorSettings.JsonLibraryVersion;
            set => Settings.CSharpGeneratorSettings.JsonLibraryVersion = value;
        }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (may generate CSharp 6 code, default: true).")]
        public bool GenerateDefaultValues
        {
            get => Settings.CSharpGeneratorSettings.GenerateDefaultValues;
            set => Settings.CSharpGeneratorSettings.GenerateDefaultValues = value;
        }

        [Argument(Name = "GenerateDataAnnotations", IsRequired = false, Description = "Specifies whether to generate data annotation attributes on DTO classes (default: true).")]
        public bool GenerateDataAnnotations
        {
            get => Settings.CSharpGeneratorSettings.GenerateDataAnnotations;
            set => Settings.CSharpGeneratorSettings.GenerateDataAnnotations = value;
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded DTO type names (must be defined in an import or other namespace).")]
        public string[] ExcludedTypeNames
        {
            get => Settings.CSharpGeneratorSettings.ExcludedTypeNames;
            set => Settings.CSharpGeneratorSettings.ExcludedTypeNames = value;
        }

        [Argument(Name = "ExcludedParameterNames", IsRequired = false, Description = "The globally excluded parameter names.")]
        public string[] ExcludedParameterNames
        {
            get => Settings.ExcludedParameterNames;
            set => Settings.ExcludedParameterNames = value;
        }

        [Argument(Name = "HandleReferences", IsRequired = false, Description = "Use preserve references handling (All) in the JSON serializer (default: false).")]
        public bool HandleReferences
        {
            get => Settings.CSharpGeneratorSettings.HandleReferences;
            set => Settings.CSharpGeneratorSettings.HandleReferences = value;
        }

        [Argument(Name = "GenerateImmutableArrayProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable array properties (default: false).")]
        public bool GenerateImmutableArrayProperties
        {
            get => Settings.CSharpGeneratorSettings.GenerateImmutableArrayProperties;
            set => Settings.CSharpGeneratorSettings.GenerateImmutableArrayProperties = value;
        }

        [Argument(Name = "GenerateImmutableDictionaryProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable dictionary properties (default: false).")]
        public bool GenerateImmutableDictionaryProperties
        {
            get => Settings.CSharpGeneratorSettings.GenerateImmutableDictionaryProperties;
            set => Settings.CSharpGeneratorSettings.GenerateImmutableDictionaryProperties = value;
        }

        [Argument(Name = "JsonSerializerSettingsTransformationMethod", IsRequired = false,
            Description = "The name of a static method which is called to transform the JsonSerializerSettings used in the generated ToJson()/FromJson() methods (default: none).")]
        public string JsonSerializerSettingsTransformationMethod
        {
            get => Settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod;
            set => Settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod = value;
        }

        [Argument(Name = "InlineNamedArrays", Description = "Inline named arrays (default: false).", IsRequired = false)]
        public bool InlineNamedArrays
        {
            get => Settings.CSharpGeneratorSettings.InlineNamedArrays;
            set => Settings.CSharpGeneratorSettings.InlineNamedArrays = value;
        }

        [Argument(Name = "InlineNamedDictionaries", Description = "Inline named dictionaries (default: false).", IsRequired = false)]
        public bool InlineNamedDictionaries
        {
            get => Settings.CSharpGeneratorSettings.InlineNamedDictionaries;
            set => Settings.CSharpGeneratorSettings.InlineNamedDictionaries = value;
        }

        [Argument(Name = "InlineNamedTuples", Description = "Inline named tuples (default: true).", IsRequired = false)]
        public bool InlineNamedTuples
        {
            get => Settings.CSharpGeneratorSettings.InlineNamedTuples;
            set => Settings.CSharpGeneratorSettings.InlineNamedTuples = value;
        }

        [Argument(Name = "InlineNamedAny", Description = "Inline named any types (default: false).", IsRequired = false)]
        public bool InlineNamedAny
        {
            get => Settings.CSharpGeneratorSettings.InlineNamedAny;
            set => Settings.CSharpGeneratorSettings.InlineNamedAny = value;
        }

        [Argument(Name = "PropertySetterAccessModifier", IsRequired = false, Description = "The access modifier of property setters (default: '').")]
        public string PropertySetterAccessModifier
        {
            get => Settings.CSharpGeneratorSettings.PropertySetterAccessModifier;
            set => Settings.CSharpGeneratorSettings.PropertySetterAccessModifier = value;
        }

        [Argument(Name = "GenerateNativeRecords", IsRequired = false, Description = "Generate C# 9.0 record types instead of record-like classes (default: false).")]
        public bool GenerateNativeRecords
        {
            get => Settings.CSharpGeneratorSettings.GenerateNativeRecords;
            set => Settings.CSharpGeneratorSettings.GenerateNativeRecords = value;
        }
        
        [Argument(Name = nameof(UseRequiredKeyword), IsRequired = false,
            Description = "Indicate whether the C# 11 'required' keyword should be used for required properties (default: false).")]
        public bool UseRequiredKeyword
        {
            get => Settings.CSharpGeneratorSettings.UseRequiredKeyword;
            set => Settings.CSharpGeneratorSettings.UseRequiredKeyword = value;
        }

        [Argument(Name = "WriteAccessor", IsRequired = false, Description = "Gets the read accessor of properties ('set' | 'init', default: 'set').")]
        public string WriteAccessor
        {
            get => Settings.CSharpGeneratorSettings.WriteAccessor;
            set => Settings.CSharpGeneratorSettings.WriteAccessor = value;
        }

        [Argument(Name = "GenerateDtoTypes", IsRequired = false, Description = "Specifies whether to generate DTO classes.")]
        public bool GenerateDtoTypes
        {
            get => Settings.GenerateDtoTypes;
            set => Settings.GenerateDtoTypes = value;
        }

        [Argument(Name = "GenerateOptionalPropertiesAsNullable", IsRequired = false, Description = "Specifies whether optional schema properties " +
            "(not required) are generated as nullable properties (default: false).")]
        public bool GenerateOptionalPropertiesAsNullable
        {
            get => Settings.CSharpGeneratorSettings.GenerateOptionalPropertiesAsNullable;
            set => Settings.CSharpGeneratorSettings.GenerateOptionalPropertiesAsNullable = value;
        }

        [Argument(Name = "GenerateNullableReferenceTypes", IsRequired = false, Description = "Specifies whether whether to " +
            "generate Nullable Reference Type annotations (default: false).")]
        public bool GenerateNullableReferenceTypes
        {
            get => Settings.CSharpGeneratorSettings.GenerateNullableReferenceTypes;
            set => Settings.CSharpGeneratorSettings.GenerateNullableReferenceTypes = value;
        }
    }
}
