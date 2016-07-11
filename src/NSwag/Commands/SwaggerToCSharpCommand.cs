using System.ComponentModel;
using NConsole;
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

        public TSettings Settings { get; set; }
        
        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName", IsRequired = false)]
        public string ClassName
        {
            get { return Settings.ClassName; }
            set { Settings.ClassName = value; }
        }

        [Description("The namespace of the generated classes.")]
        [Argument(Name = "Namespace")]
        public string Namespace
        {
            get { return Settings.CSharpGeneratorSettings.Namespace; }
            set { Settings.CSharpGeneratorSettings.Namespace = value; }
        }

        [Description("The additional namespace usages.")]
        [Argument(Name = "AdditionalNamespaceUsages", IsRequired = false)]
        public string[] AdditionalNamespaceUsages
        {
            get { return Settings.AdditionalNamespaceUsages; }
            set { Settings.AdditionalNamespaceUsages = value; }
        }

        [Description("Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        [Argument(Name = "RequiredPropertiesMustBeDefined", IsRequired = false)]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined; }
            set { Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = value; }
        }

        [Description("The date time .NET type (default: 'DateTime').")]
        [Argument(Name = "DateTimeType", IsRequired = false)]
        public string DateTimeType
        {
            get { return Settings.CSharpGeneratorSettings.DateTimeType; }
            set { Settings.CSharpGeneratorSettings.DateTimeType = value; }
        }

        [Description("The generic array .NET type (default: 'ObservableCollection').")]
        [Argument(Name = "ArrayType", IsRequired = false)]
        public string ArrayType
        {
            get { return Settings.CSharpGeneratorSettings.ArrayType; }
            set { Settings.CSharpGeneratorSettings.ArrayType = value; }
        }

        [Description("The generic dictionary .NET type (default: 'Dictionary').")]
        [Argument(Name = "DictionaryType", IsRequired = false)]
        public string DictionaryType
        {
            get { return Settings.CSharpGeneratorSettings.DictionaryType; }
            set { Settings.CSharpGeneratorSettings.DictionaryType = value; }
        }

        [Description("The CSharp class style, 'Poco' or 'Inpc' (default: 'Inpc').")]
        [Argument(Name = "ClassStyle", IsRequired = false)]
        public CSharpClassStyle ClassStyle
        {
            get { return Settings.CSharpGeneratorSettings.ClassStyle; }
            set { Settings.CSharpGeneratorSettings.ClassStyle = value; }
        }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", IsRequired = false)]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }
    }
}