using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands
{
    [Command(Name = "list-types", Description = "List all types for the given assembly and settings.")]
    public abstract class ListTypesCommandBase : AssemblyOutputCommandBase
    {
        [Argument(Name = "File", IsRequired = false, Description = "The nswag.json configuration file path.")]
        public string File { get; set; }

        [Argument(Name = "Assembly", IsRequired = false, Description = "The path to the Web API .NET assembly.")]
        public string AssemblyPath
        {
            get { return Settings.AssemblyPath; }
            set { Settings.AssemblyPath = value; }
        }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await Task.Run(async () =>
            {
                var generator = await CreateGeneratorAsync();
                var classNames = generator.GetExportedClassNames();

                host.WriteMessage("\r\n");
                foreach (var className in classNames)
                    host.WriteMessage(className + "\r\n");
                host.WriteMessage("\r\n");

                return classNames;
            });
        }
    }
}