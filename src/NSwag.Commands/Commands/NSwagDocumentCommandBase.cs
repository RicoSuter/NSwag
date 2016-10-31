//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands
{
    [Command(Name = "run", Description = "Executes an .nswag file.")]
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
                var files = DynamicApis.DirectoryGetFiles(DynamicApis.DirectoryGetCurrentDirectory(), "*.nswag");
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

        /// <summary>Loads an existing NSwagDocument.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The document.</returns>
        protected abstract Task<NSwagDocumentBase> LoadDocumentAsync(string filePath);
    }
}
