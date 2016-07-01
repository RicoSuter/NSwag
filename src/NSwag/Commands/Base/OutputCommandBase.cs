using System;
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
        [Argument(Name = "Output", IsRequired = false)]
        public string OutputFilePath { get; set; }

        public abstract Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected bool TryWriteFileOutput(IConsoleHost host, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(OutputFilePath))
            {
                var file = new FileInfo(OutputFilePath);
                var directory = file.Directory;

                if (!directory.Exists)
                    directory.Create();

                File.WriteAllText(OutputFilePath, generator(), Encoding.UTF8);
                host?.WriteMessage("Code has been successfully written to file.\n");

                return true; 
            }
            return false;
        }
    }
}