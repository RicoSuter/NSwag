using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands.Base
{
    public abstract class OutputCommandBase : IConsoleCommand
    {
        [Display(Description = "The output file path (optional).")]
        [Argument(Name = "Output", IsRequired = false)]
        public string OutputFilePath { get; set; }

        public abstract Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected bool TryWriteFileOutput(IConsoleHost host, Func<string> generator)
        {
            return TryWriteFileOutput(OutputFilePath, host, generator);
        }

        protected bool TryWriteFileOutput(string path, IConsoleHost host, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var file = new FileInfo(path);
                var directory = file.Directory;

                if (!directory.Exists)
                    directory.Create();

                File.WriteAllText(path, generator(), Encoding.UTF8);
                host?.WriteMessage("Code has been successfully written to file.\n");

                return true; 
            }
            return false;
        }
    }
}