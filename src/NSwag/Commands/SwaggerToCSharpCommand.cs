using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.ClientGenerators;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpCommand : InputOutputCommandBase
    {
        [Description("The class name of the generated client.")]
        [Argument(Name = "ClassName")]
        public string ClassName { get; set; }

        [Description("The namespace of the generated classes.")]
        [Argument(Name = "Namespace")]
        public string Namespace { get; set; }

        [Description("Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", DefaultValue = true)]
        public bool GenerateClientClasses { get; set; }

        [Description("Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", DefaultValue = false)]
        public bool GenerateClientInterfaces { get; set; }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", DefaultValue = true)]
        public bool GenerateDtoTypes { get; set; }

        [Description("The client base class (empty for no base class).")]
        [Argument(Name = "ClientBaseClass", DefaultValue = "")]
        public string ClientBaseClass { get; set; }

        [Description("Specifies whether to call CreateHttpClientAsync on the base class to create a new HttpClient.")]
        [Argument(Name = "UseHttpClientCreationMethod", DefaultValue = false)]
        public bool UseHttpClientCreationMethod { get; set; }
        
        [Description("The operation generation mode ('SingleClientFromOperationId' or 'MultipleClientsFromPathSegments').")]
        [Argument(Name = "OperationGenerationMode", DefaultValue = OperationGenerationMode.SingleClientFromOperationId)]
        public OperationGenerationMode OperationGenerationMode { get; set; }

        [Description("Specifies whether a required property must be defined in JSON (sets Required.Always when the property is required).")]
        [Argument(Name = "RequiredPropertiesMustBeDefined", DefaultValue = true)]
        public bool RequiredPropertiesMustBeDefined { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var settings = new SwaggerToCSharpGeneratorSettings
            {
                ClassName = ClassName,
                OperationGenerationMode = OperationGenerationMode,

                UseHttpClientCreationMethod = UseHttpClientCreationMethod,

                GenerateClientClasses = GenerateClientClasses, 
                GenerateClientInterfaces = GenerateClientInterfaces, 
                ClientBaseClass = ClientBaseClass, 
                GenerateDtoTypes = GenerateDtoTypes
            };

            settings.CSharpGeneratorSettings.Namespace = Namespace;
            settings.CSharpGeneratorSettings.RequiredPropertiesMustBeDefined = RequiredPropertiesMustBeDefined;

            var clientGenerator = new SwaggerToCSharpGenerator(InputSwaggerService, settings);

            var output = clientGenerator.GenerateFile();
            WriteOutput(host, output);
        }
    }
}