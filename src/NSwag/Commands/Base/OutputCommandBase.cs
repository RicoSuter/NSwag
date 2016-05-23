using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands.Base
{
    public abstract class OutputCommandBase : IConsoleCommand
    {
        [Description("The output file path (optional).")]
        [Argument(Name = "Output", DefaultValue = "")]
        public string OutputFilePath { get; set; }

        public abstract Task RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected void WriteOutput(IConsoleHost host, string output)
        {
            if (string.IsNullOrEmpty(OutputFilePath))
                host.WriteMessage(output);
            else
            {
                var file = new FileInfo(OutputFilePath);
                var directory = file.Directory;

                if (!directory.Exists)
                    directory.Create();

                File.WriteAllText(OutputFilePath, output, Encoding.UTF8);
                host.WriteMessage("Code has been successfully written to file.\n");
            }
        }
    }
}