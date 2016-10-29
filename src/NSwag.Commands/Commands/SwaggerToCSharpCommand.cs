using System.ComponentModel.DataAnnotations;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.CodeGenerators;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.Commands.Base;

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
        
        [Display(Description = "The class name of the generated client.")]
        [Argument(Name = "ClassName", IsRequired = false)]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Display(Description = "The namespace of the generated classes.")]
        [Argument(Name = "Namespace")]
        public string Namespace
        {
            get { return Settings.CSharpGeneratorSettings.Namespace; }
            set { Settings.CSharpGeneratorSettings.Namespace = value; }
        }

        [Display(Description = "The additional namespace usages.")]
        [Argument(Name = "AdditionalNamespaceUsages", IsRequired = false)]
        public string[] AdditionalNamespaceUsages
        {
            get { return Settings.AdditionalNamespaceUsages; }
            set { Settings.AdditionalNamespaceUsages = value; }
        }

        [Display(Description = "Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false)]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined; }
            set { Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = value; }
        }

        [Display(Description = "The date .NET type (default: 'DateTime').")]
        [Argument(Name = "DateType", IsRequired = false)]
        public string DateType
        {
            get { return Settings.CSharpGeneratorSettings.DateType; }
            set { Settings.CSharpGeneratorSettings.DateType = value; }
        }

        [Display(Description = "The date time .NET type (default: 'DateTime').")]
        [Argument(Name = "DateTimeType", IsRequired = false)]
        public string DateTimeType
        {
            get { return Settings.CSharpGeneratorSettings.DateTimeType; }
            set { Settings.CSharpGeneratorSettings.DateTimeType = value; }
        }

        [Display(Description = "The time .NET type (default: 'TimeSpan').")]
        [Argument(Name = "TimeType", IsRequired = false)]
        public string TimeType
        {
            get { return Settings.CSharpGeneratorSettings.TimeType; }
            set { Settings.CSharpGeneratorSettings.TimeType = value; }
        }

        [Display(Description = "The time span .NET type (default: 'TimeSpan').")]
        [Argument(Name = "TimeSpanType", IsRequired = false)]
        public string TimeSpanType
        {
            get { return Settings.CSharpGeneratorSettings.TimeSpanType; }
            set { Settings.CSharpGeneratorSettings.TimeSpanType = value; }
        }

        [Display(Description = "The generic array .NET type (default: 'ObservableCollection').")]
        [Argument(Name = "ArrayType", IsRequired = false)]
        public string ArrayType
        {
            get { return Settings.CSharpGeneratorSettings.ArrayType; }
            set { Settings.CSharpGeneratorSettings.ArrayType = value; }
        }

        [Display(Description = "The generic dictionary .NET type (default: 'Dictionary').")]
        [Argument(Name = "DictionaryType", IsRequired = false)]
        public string DictionaryType
        {
            get { return Settings.CSharpGeneratorSettings.DictionaryType; }
            set { Settings.CSharpGeneratorSettings.DictionaryType = value; }
        }

        [Display(Description = "The CSharp class style, 'Poco' or 'Inpc' (default: 'Inpc').")]
        [Argument(Name = "ClassStyle", IsRequired = false)]
        public CSharpClassStyle ClassStyle
        {
            get { return Settings.CSharpGeneratorSettings.ClassStyle; }
            set { Settings.CSharpGeneratorSettings.ClassStyle = value; }
        }

        [Display(Description = "The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", IsRequired = false)]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Display(Description = "Specifies whether to generate default values for properties (may generate CSharp 6 code, default: true).")]
        [Argument(Name = "GenerateDefaultValues", IsRequired = false)]
        public bool GenerateDefaultValues
        {
            get { return Settings.CSharpGeneratorSettings.GenerateDefaultValues; }
            set { Settings.CSharpGeneratorSettings.GenerateDefaultValues = value; }
        }
    }
}
