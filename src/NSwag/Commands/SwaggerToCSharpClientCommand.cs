using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.Commands
{
    [Description("Generates CSharp client code from a Swagger specification.")]
    public class SwaggerToCSharpClientCommand : SwaggerToCSharpCommand<SwaggerToCSharpClientGeneratorSettings>
    {
        public SwaggerToCSharpClientCommand() : base(new SwaggerToCSharpClientGeneratorSettings())
        {
        }

        [Description("The client base class (empty for no base class).")]
        [Argument(Name = "ClientBaseClass", IsRequired = false)]
        public string ClientBaseClass
        {
            get { return Settings.ClientBaseClass; }
            set { Settings.ClientBaseClass = value; }
        }

        [Description("Specifies whether generate client classes.")]
        [Argument(Name = "GenerateClientClasses", IsRequired = false)]
        public bool GenerateClientClasses
        {
            get { return Settings.GenerateClientClasses; }
            set { Settings.GenerateClientClasses = value; }
        }

        [Description("Specifies whether generate interfaces for the client classes.")]
        [Argument(Name = "GenerateClientInterfaces", IsRequired = false)]
        public bool GenerateClientInterfaces
        {
            get { return Settings.GenerateClientInterfaces; }
            set { Settings.GenerateClientInterfaces = value; }
        }

        [Description("Specifies whether to generate DTO classes.")]
        [Argument(Name = "GenerateDtoTypes", IsRequired = false)]
        public bool GenerateDtoTypes
        {
            get { return Settings.GenerateDtoTypes; }
            set { Settings.GenerateDtoTypes = value; }
        }

        [Description("Specifies whether to call CreateHttpClientAsync on the base class to create a new HttpClient.")]
        [Argument(Name = "UseHttpClientCreationMethod", IsRequired = false)]
        public bool UseHttpClientCreationMethod
        {
            get { return Settings.UseHttpClientCreationMethod; }
            set { Settings.UseHttpClientCreationMethod = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = Run();
            WriteFileOutput(host, () => code);
            return code;
        }

        public string Run()
        {
            var clientGenerator = new SwaggerToCSharpClientGenerator(InputSwaggerService, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}