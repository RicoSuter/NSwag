using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpCommand : InputOutputCommandBase
    {
        public SwaggerToCSharpCommand()
        {
            Settings = new SwaggerToCSharpGeneratorSettings();
            Namespace = "MyNamespace";
        }

        [JsonIgnore]
        public SwaggerToCSharpGeneratorSettings Settings { get; set; }

        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName")]
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

        [Description("Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", DefaultValue = true)]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Description("Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", DefaultValue = false)]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", DefaultValue = true)]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Description("The client base class (empty for no base class).")]
        [Argument(Name = "ClientBaseClass", DefaultValue = "")]
        public string ClientBaseClass
        {
            get { return Settings.ClientBaseClass; }
            set { Settings.ClientBaseClass = value; }
        }

        [Description("Specifies whether to call CreateHttpClientAsync on the base class to create a new HttpClient.")]
        [Argument(Name = "UseHttpClientCreationMethod", DefaultValue = false)]
        public bool UseHttpClientCreationMethod
        {
            get { return Settings.UseHttpClientCreationMethod; }
            set { Settings.UseHttpClientCreationMethod = value; }
        }

        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode
        {
            get { return Settings.OperationGenerationMode; }
            set { Settings.OperationGenerationMode = value; }
        }

        [Description("Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        [Argument(Name = "RequiredPropertiesMustBeDefined", DefaultValue = true)]
        public bool RequiredPropertiesMustBeDefined
        {
            get { return Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined; }
            set { Settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = value; }
        }

        [Description("The additional namespace usages.")]
        [Argument(Name = "AdditionalNamespaceUsages", DefaultValue = null)]
        public string[] AdditionalNamespaceUsages
        {
            get { return Settings.AdditionalNamespaceUsages; }
            set { Settings.AdditionalNamespaceUsages = value; }
        }

        [Description("The date time .NET type (DateTime or DateTimeOffset).")]
        [Argument(Name = "DateTimeType", DefaultValue = CSharpDateTimeType.DateTime)]
        public CSharpDateTimeType DateTimeType
        {
            get { return Settings.CSharpGeneratorSettings.DateTimeType; }
            set { Settings.CSharpGeneratorSettings.DateTimeType = value; }
        }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var output = await RunAsync();
            WriteOutput(host, output);
        }

        public async Task<string> RunAsync()
        {
            var clientGenerator = new SwaggerToCSharpGenerator(InputSwaggerService, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}