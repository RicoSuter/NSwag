//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    public abstract class SwaggerToCSharpCommand<TSettings> : InputOutputCommandBase
         where TSettings : SwaggerToCSharpGeneratorSettings
    {
        protected SwaggerToCSharpCommand(TSettings settings)
        {
            Settings = settings; 
        }

        [JsonIgnore]
        public TSettings Settings { get; set; }
        
        [Argument(Name = "ClassName", IsRequired = false, Description = "The class name of the generated client.")]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Argument(Name = "Namespace", Description = "The namespace of the generated classes.")]
        public string Namespace
        {
            get { return Settings.CSharpGeneratorSettings.Namespace; }
            set { Settings.CSharpGeneratorSettings.Namespace = value; }
        }

        [Argument(Name = "AdditionalNamespaceUsages", IsRequired = false, Description = "The additional namespace usages.")]
        public string[] AdditionalNamespaceUsages
        {
            get { return Settings.AdditionalNamespaceUsages; }
            set { Settings.AdditionalNamespaceUsages = value; }
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

        [Argument(Name = "ClassStyle", IsRequired = false, Description = "The CSharp class style, 'Poco' or 'Inpc' (default: 'Inpc').")]
        public CSharpClassStyle ClassStyle
        {
            get { return Settings.CSharpGeneratorSettings.ClassStyle; }
            set { Settings.CSharpGeneratorSettings.ClassStyle = value; }
        }

        [Argument(Name = "OperationGenerationMode", IsRequired = false, Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Argument(Name = "GenerateDefaultValues", IsRequired = false, Description = "Specifies whether to generate default values for properties (may generate CSharp 6 code, default: true).")]
        public bool GenerateDefaultValues
        {
            get { return Settings.CSharpGeneratorSettings.GenerateDefaultValues; }
            set { Settings.CSharpGeneratorSettings.GenerateDefaultValues = value; }
        }
    }
}
