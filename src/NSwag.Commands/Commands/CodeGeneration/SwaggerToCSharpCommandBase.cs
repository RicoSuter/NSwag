//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CSharp;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
{
    public abstract class SwaggerToCSharpCommandBase<TSettings> : CodeGeneratorCommandBase<TSettings>
         where TSettings : SwaggerToCSharpGeneratorSettings
    {
        protected SwaggerToCSharpCommandBase(TSettings settings)
            : base(settings)
        {
        }

        [Argument(Name = "ClassName", IsRequired = false, Description = "The class name of the generated client.")]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Argument(Name = "OperationGenerationMode", IsRequired = false, Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return OperationGenerationModeConverter.GetOperationGenerationMode(Settings.OperationNameGenerator); }
            set { Settings.OperationNameGenerator = OperationGenerationModeConverter.GetOperationNameGenerator(value); }
        }

        [Argument(Name = "AdditionalNamespaceUsages", IsRequired = false, Description = "The additional namespace usages.")]
        public string[] AdditionalNamespaceUsages
        {
            get { return Settings.AdditionalNamespaceUsages; }
            set { Settings.AdditionalNamespaceUsages = value; }
        }

        [Argument(Name = "AdditionalContractNamespaceUsages", IsRequired = false, Description = "The additional contract namespace usages.")]
        public string[] AdditionalContractNamespaceUsages
        {
            get { return Settings.AdditionalContractNamespaceUsages; }
            set { Settings.AdditionalContractNamespaceUsages = value; }
        }

        [Argument(Name = "GenerateOptionalParameters", IsRequired = false,
                  Description = "Specifies whether to reorder parameters (required first, optional at the end) and generate optional parameters (default: false).")]
        public bool GenerateOptionalParameters
        {
            get { return Settings.GenerateOptionalParameters; }
            set { Settings.GenerateOptionalParameters = value; }
        }

        [Argument(Name = "GenerateJsonMethods", IsRequired = false,
            Description = "Specifies whether to render ToJson() and FromJson() methods for DTOs (default: true).")]
        public bool GenerateJsonMethods
        {
            get { return Settings.CSharpGeneratorSettings.GenerateJsonMethods; }
            set { Settings.CSharpGeneratorSettings.GenerateJsonMethods = value; }
        }

        [Argument(Name = "ParameterArrayType", IsRequired = false, Description = "The generic array .NET type of operation parameters (default: 'IEnumerable').")]
        public string ParameterArrayType
        {
            get { return Settings.ParameterArrayType; }
            set { Settings.ParameterArrayType = value; }
        }

        [Argument(Name = "ParameterDictionaryType", IsRequired = false, Description = "The generic dictionary .NET type of operation parameters (default: 'IReadOnlyDictionary').")]
        public string ParameterDictionaryType
        {
            get { return Settings.ParameterDictionaryType; }
            set { Settings.ParameterDictionaryType = value; }
        }

        [Argument(Name = "ResponseArrayType", IsRequired = false, Description = "The generic array .NET type of operation responses (default: 'ICollection').")]
        public string ResponseArrayType
        {
            get { return Settings.ResponseArrayType; }
            set { Settings.ResponseArrayType = value; }
        }

        [Argument(Name = "ResponseDictionaryType", IsRequired = false, Description = "The generic dictionary .NET type of operation responses (default: 'IDictionary').")]
        public string ResponseDictionaryType
        {
            get { return Settings.ResponseDictionaryType; }
            set { Settings.ResponseDictionaryType = value; }
        }

        [Argument(Name = "WrapResponses", IsRequired = false, Description = "Specifies whether to wrap success responses to allow full response access.")]
        public bool WrapResponses
        {
            get { return Settings.WrapResponses; }
            set { Settings.WrapResponses = value; }
        }

        [Argument(Name = "WrapResponseMethods", IsRequired = false, Description = "List of methods where responses are wrapped ('ControllerName.MethodName', WrapResponses must be true).")]
        public string[] WrapResponseMethods
        {
            get { return Settings.WrapResponseMethods; }
            set { Settings.WrapResponseMethods = value; }
        }

        [Argument(Name = "GenerateResponseClasses", IsRequired = false, Description = "Specifies whether to generate response classes (default: true).")]
        public bool GenerateResponseClasses
        {
            get { return Settings.GenerateResponseClasses; }
            set { Settings.GenerateResponseClasses = value; }
        }

        [Argument(Name = "ResponseClass", IsRequired = false, Description = "The response class (default 'SwaggerResponse', may use '{controller}' placeholder).")]
        public string ResponseClass
        {
            get { return Settings.ResponseClass; }
            set { Settings.ResponseClass = value; }
        }

        // CSharpGeneratorSettings

        [Argument(Name = "Namespace", Description = "The namespace of the generated classes.")]
        public string Namespace
        {
            get { return Settings.CSharpGeneratorSettings.Namespace; }
            set { Settings.CSharpGeneratorSettings.Namespace = value; }
        }

        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false,
                  Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined; }
            set { Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = value; }
        }

        [Argument(Name = "DateType", IsRequired = false, Description = "The date .NET type (default: 'DateTime').")]
        public string DateType
        {
            get { return Settings.CSharpGeneratorSettings.DateType; }
            set { Settings.CSharpGeneratorSettings.DateType = value; }
        }

        [Argument(Name = "JsonConverters", IsRequired = false, Description = "Specifies the custom Json.NET converter types (optional, comma separated).")]
        public string[] JsonConverters
        {
            get { return Settings.CSharpGeneratorSettings.JsonConverters; }
            set { Settings.CSharpGeneratorSettings.JsonConverters = value; }
        }

        [Argument(Name = "DateTimeType", IsRequired = false, Description = "The date time .NET type (default: 'DateTime').")]
        public string DateTimeType
        {
            get { return Settings.CSharpGeneratorSettings.DateTimeType; }
            set { Settings.CSharpGeneratorSettings.DateTimeType = value; }
        }

        [Argument(Name = "TimeType", IsRequired = false, Description = "The time .NET type (default: 'TimeSpan').")]
        public string TimeType
        {
            get { return Settings.CSharpGeneratorSettings.TimeType; }
            set { Settings.CSharpGeneratorSettings.TimeType = value; }
        }

        [Argument(Name = "TimeSpanType", IsRequired = false, Description = "The time span .NET type (default: 'TimeSpan').")]
        public string TimeSpanType
        {
            get { return Settings.CSharpGeneratorSettings.TimeSpanType; }
            set { Settings.CSharpGeneratorSettings.TimeSpanType = value; }
        }

        [Argument(Name = "ArrayType", IsRequired = false, Description = "The generic array .NET type (default: 'ObservableCollection').")]
        public string ArrayType
        {
            get { return Settings.CSharpGeneratorSettings.ArrayType; }
            set { Settings.CSharpGeneratorSettings.ArrayType = value; }
        }

        [Argument(Name = "DictionaryType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryType
        {
            get { return Settings.CSharpGeneratorSettings.DictionaryType; }
            set { Settings.CSharpGeneratorSettings.DictionaryType = value; }
        }

        [Argument(Name = "ArrayBaseType", IsRequired = false, Description = "The generic array .NET type (default: 'ObservableCollection').")]
        public string ArrayBaseType
        {
            get { return Settings.CSharpGeneratorSettings.ArrayBaseType; }
            set { Settings.CSharpGeneratorSettings.ArrayBaseType = value; }
        }

        [Argument(Name = "DictionaryBaseType", IsRequired = false, Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        public string DictionaryBaseType
        {
            get { return Settings.CSharpGeneratorSettings.DictionaryBaseType; }
            set { Settings.CSharpGeneratorSettings.DictionaryBaseType = value; }
        }

        [Argument(Name = "ClassStyle", IsRequired = false, Description = "The CSharp class style, 'Poco' or 'Inpc' (default: 'Inpc').")]
        public CSharpClassStyle ClassStyle
        {
            get { return Settings.CSharpGeneratorSettings.ClassStyle; }
            set { Settings.CSharpGeneratorSettings.ClassStyle = value; }
        }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (may generate CSharp 6 code, default: true).")]
        public bool GenerateDefaultValues
        {
            get { return Settings.CSharpGeneratorSettings.GenerateDefaultValues; }
            set { Settings.CSharpGeneratorSettings.GenerateDefaultValues = value; }
        }

        [Argument(Name = "GenerateDataAnnotations", IsRequired = false, Description = "Specifies whether to generate data annotation attributes on DTO classes (default: true).")]
        public bool GenerateDataAnnotations
        {
            get { return Settings.CSharpGeneratorSettings.GenerateDataAnnotations; }
            set { Settings.CSharpGeneratorSettings.GenerateDataAnnotations = value; }
        }

        [Argument(Name = "ExcludedTypeNames", IsRequired = false, Description = "The excluded DTO type names (must be defined in an import or other namespace).")]
        public string[] ExcludedTypeNames
        {
            get { return Settings.CSharpGeneratorSettings.ExcludedTypeNames; }
            set { Settings.CSharpGeneratorSettings.ExcludedTypeNames = value; }
        }

        [Argument(Name = "HandleReferences", IsRequired = false, Description = "Use preserve references handling (All) in the JSON serializer (default: false).")]
        public bool HandleReferences
        {
            get { return Settings.CSharpGeneratorSettings.HandleReferences; }
            set { Settings.CSharpGeneratorSettings.HandleReferences = value; }
        }

        [Argument(Name = "GenerateImmutableArrayProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable array properties (default: false).")]
        public bool GenerateImmutableArrayProperties
        {
            get { return Settings.CSharpGeneratorSettings.GenerateImmutableArrayProperties; }
            set { Settings.CSharpGeneratorSettings.GenerateImmutableArrayProperties = value; }
        }

        [Argument(Name = "GenerateImmutableDictionaryProperties", IsRequired = false,
                  Description = "Specifies whether to remove the setter for non-nullable dictionary properties (default: false).")]
        public bool GenerateImmutableDictionaryProperties
        {
            get { return Settings.CSharpGeneratorSettings.GenerateImmutableDictionaryProperties; }
            set { Settings.CSharpGeneratorSettings.GenerateImmutableDictionaryProperties = value; }
        }

        [Argument(Name = "JsonSerializerSettingsTransformationMethod", IsRequired = false,
            Description = "The name of a static method which is called to transform the JsonSerializerSettings used in the generated ToJson()/FromJson() methods (default: none).")]
        public string JsonSerializerSettingsTransformationMethod
        {
            get { return Settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod; }
            set { Settings.CSharpGeneratorSettings.JsonSerializerSettingsTransformationMethod = value; }
        }
    }
}
