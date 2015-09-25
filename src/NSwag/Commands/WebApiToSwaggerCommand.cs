using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.Commands
{
    public class WebApiToSwaggerCommand : OutputCommandBase
    {
        [Description("The path to the Web API .NET assembly.")]
        [Argument(Name = "Assembly")]
        public string Assembly { get; set; }

        [Description("The Web API controller full class name.")]
        [Argument(Name = "Controller")]
        public string Controller { get; set; }

        [Description("The Web API default URL template.")]
        [Argument(Name = "UrlTemplate", DefaultValue = "api/{controller}/{action}/{id}")]
        public string UrlTemplate { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var generator = new WebApiAssemblyToSwaggerGenerator(Assembly);
            var service = generator.Generate(Controller, UrlTemplate);
            var json = service.ToJson();

            WriteOutput(host, json);
        }
    }
}