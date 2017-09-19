using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands
{
    [Command(Name = "list-controllers", Description = "List all controllers classes for the given assembly and settings.")]
    public abstract class ListWebApiControllersCommandBase : WebApiAssemblyOutputCommandBase
    {
        [Argument(Name = "File", IsRequired = false, Description = "The nswag.json configuration file path.")]
        public string File { get; set; }

        [Argument(Name = "Assembly", IsRequired = false, Description = "The path or paths to the Web API .NET assemblies (comma separated).")]
        public string[] AssemblyPaths
        {
            get { return Settings.AssemblyPaths; }
            set { Settings.AssemblyPaths = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await Task.Run(async () =>
            {
                var generator = await CreateGeneratorAsync();
                var controllers = generator.GetExportedControllerClassNames();

                host.WriteMessage("\r\n");
                foreach (var controller in controllers)
                    host.WriteMessage(controller + "\r\n");
                host.WriteMessage("\r\n");

                return controllers;
            });
        }
    }
}