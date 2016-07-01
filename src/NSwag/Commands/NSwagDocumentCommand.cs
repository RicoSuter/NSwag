using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands
{
    [Description("Executes an .nswag file.")]
    public class NSwagDocumentCommand : IConsoleCommand
    {
        [Argument(Position = 0, IsRequired = false)]
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
            host.WriteMessage("Executing file '" + filePath + "'...\n");

            var document = NSwagDocument.LoadDocument(filePath);
            await document.ExecuteAsync();

            host.WriteMessage("Done.\n");
        }
    }
}
