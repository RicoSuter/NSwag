using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.Commands
{
    [Description("Generates CSharp Web API controller code from a Swagger specification.")]
    public class SwaggerToCSharpWebApiControllerCommand : SwaggerToCSharpCommand<SwaggerToCSharpWebApiControllerGeneratorSettings>
    {
        public SwaggerToCSharpWebApiControllerCommand() : base(new SwaggerToCSharpWebApiControllerGeneratorSettings())
        {
        }
        
        [Description("The controller base class (empty for 'ApiController').")]
        [Argument(Name = "ControllerBaseClass", DefaultValue = "")]
        public string ControllerBaseClass
        {
            get { return Settings.ControllerBaseClass; }
            set { Settings.ControllerBaseClass = value; }
        }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var output = await RunAsync();
            WriteOutput(host, output);
        }

        public async Task<string> RunAsync()
        {
            var clientGenerator = new SwaggerToCSharpWebApiControllerGenerator(InputSwaggerService, Settings);
            return clientGenerator.GenerateFile();
        }
    }
}