using System.ComponentModel;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    [Description("Generates a Swagger specification for a controller or controlles contained in a .NET Web API assembly.")]
    public class WebApiToSwaggerCommand : OutputCommandBase
    {
        [Description("The path to the Web API .NET assembly.")]
        [Argument(Name = "Assembly")]
        public string Assembly { get; set; }

        [Description("The Web API controller full class name or empty to load all controllers from the assembly.")]
        [Argument(Name = "Controller", DefaultValue = "")]
        public string Controller { get; set; }

        [Description("The Web API default URL template.")]
        [Argument(Name = "DefaultUrlTemplate", DefaultValue = "api/{controller}/{action}/{id}")]
        public string DefaultUrlTemplate { get; set; }

        [Description("The default enum handling ('String' or 'Integer').")]
        [Argument(Name = "DefaultEnumHandling", DefaultValue = EnumHandling.String)]
        public EnumHandling DefaultEnumHandling { get; set; }

        public override async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var generator = new WebApiAssemblyToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings
            {
                AssemblyPath = Assembly,
                DefaultUrlTemplate = DefaultUrlTemplate, 
                DefaultEnumHandling = DefaultEnumHandling
            });

            var service = string.IsNullOrEmpty(Controller) ? 
                generator.GenerateForAssemblyControllers() : 
                generator.GenerateForSingleController(Controller);

            var json = service.ToJson();

            WriteOutput(host, json);
        }
    }
}