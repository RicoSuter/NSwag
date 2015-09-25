using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands
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
                File.WriteAllText(OutputFilePath, output, Encoding.UTF8);
                host.WriteMessage("Client code successfully written to file.");
            }
        }
    }
}