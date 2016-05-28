using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.CodeGenerators.CSharp;

namespace NSwag.Commands
{
    [Description("Generates CSharp Web API controller code from a Swagger specification.")]
    public class SwaggerToCSharpControllerCommand : SwaggerToCSharpCommand<SwaggerToCSharpWebApiControllerGeneratorSettings>
    {
        public SwaggerToCSharpControllerCommand() : base(new SwaggerToCSharpWebApiControllerGeneratorSettings())
        {
        }

        [Description("The controller base class (empty for 'ApiController').")]
        [Argument(Name = "ControllerBaseClass", IsRequired = false)]
        public string ControllerBaseClass
        {
            get { return Settings.ControllerBaseClass; }
            set { Settings.ControllerBaseClass = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var code = await RunAsync();
            TryWriteFileOutput(host, () => code);
            return code;
        }

        public async Task<string> RunAsync()
        {
            return await Task.Run(() =>
            {
                var clientGenerator = new SwaggerToCSharpWebApiControllerGenerator(InputSwaggerService, Settings);
                return clientGenerator.GenerateFile();
            });
        }
    }
}