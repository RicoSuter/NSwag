//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands.Document
{
    [Command(Name = "run", Description = "Executes an .nswag file. If 'input' is not specified then all *.nswag files and the nswag.json file is executed.")]
    public class ExecuteDocumentCommand : IConsoleCommand
    {
        [Argument(Position = 1, IsRequired = false)]
        public string Input { get; set; }

        [Argument(Name = nameof(Variables), IsRequired = false)]
        public string Variables { get; set; }

        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (!string.IsNullOrEmpty(Input) && !Input.StartsWith("/") && !Input.StartsWith("-"))
            {
                await ExecuteDocumentAsync(host, Input);
            }
            else
            {
                var hasNSwagJson = File.Exists("nswag.json");
                if (hasNSwagJson)
                {
                    await ExecuteDocumentAsync(host, "nswag.json");
                }

                var currentDirectory = Directory.GetCurrentDirectory();
                var files = Directory.GetFiles(currentDirectory, "*.nswag");
                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        await ExecuteDocumentAsync(host, file);
                    }
                }
                else if (!hasNSwagJson)
                {
                    host.WriteMessage("Current directory does not contain any .nswag files.");
                }
            }
            return null;
        }

        private async Task ExecuteDocumentAsync(IConsoleHost host, string filePath)
        {
            host.WriteMessage("\nExecuting file '" + filePath + "' with variables '" + Variables + "'...\n");

            var document = await NSwagDocument.LoadWithTransformationsAsync(filePath, Variables);
            if (document.Runtime != Runtime.Default)
            {
                if (document.Runtime != RuntimeUtilities.CurrentRuntime)
                {
                    throw new InvalidOperationException("The specified runtime in the document (" + document.Runtime + ") differs " +
                                                        "from the current process runtime (" + RuntimeUtilities.CurrentRuntime + "). " +
                                                        "Change the runtime with the '/runtime:" + document.Runtime + "' parameter " +
                                                        "or run the file with the correct command line binary.");
                }
            }

            await document.ExecuteAsync();
            host.WriteMessage("Done.\n");
        }
    }
}
