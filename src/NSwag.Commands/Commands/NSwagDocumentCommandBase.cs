using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands
{
    //[Display(Description = "Executes an .nswag file.")]
    public abstract class NSwagDocumentCommandBase : IConsoleCommand
    {
        [Argument(Position = 1, IsRequired = false)]
        public string Input { get; set; }

        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (!string.IsNullOrEmpty(Input))
                await ExecuteDocumentAsync(host, Input);
            else
            {
                var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.nswag");
                if (files.Any())
                {
                    foreach (var file in files)
                        await ExecuteDocumentAsync(host, file);
                }
                else
                    host.WriteMessage("Current directory does not contain any .nswag files.");
            }
            return null; 
        }

        private async Task ExecuteDocumentAsync(IConsoleHost host, string filePath)
        {
            host.WriteMessage("\nExecuting file '" + filePath + "'...\n");

            var document = await LoadDocumentAsync(filePath);
            await document.ExecuteAsync();

            host.WriteMessage("Done.\n");
        }

        protected abstract Task<NSwagDocumentBase> LoadDocumentAsync(string filePath);
    }
}
