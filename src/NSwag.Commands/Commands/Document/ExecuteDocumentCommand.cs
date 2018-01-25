//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
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
                await ExecuteDocumentAsync(host, Input);
            else
            {
                var hasNSwagJson = await DynamicApis.FileExistsAsync("nswag.json").ConfigureAwait(false);
                if (hasNSwagJson)
                    await ExecuteDocumentAsync(host, "nswag.json");

                var currentDirectory = await DynamicApis.DirectoryGetCurrentDirectoryAsync().ConfigureAwait(false);
                var files = await DynamicApis.DirectoryGetFilesAsync(currentDirectory, "*.nswag").ConfigureAwait(false);
                if (files.Any())
                {
                    foreach (var file in files)
                        await ExecuteDocumentAsync(host, file);
                }
                else if (!hasNSwagJson)
                    host.WriteMessage("Current directory does not contain any .nswag files.");
            }
            return null;
        }

        private async Task ExecuteDocumentAsync(IConsoleHost host, string filePath)
        {
            host.WriteMessage("\nExecuting file '" + filePath + "'...\n");

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

                if (document.SelectedSwaggerGenerator == document.SwaggerGenerators.WebApiToSwaggerCommand &&
                    document.SwaggerGenerators.WebApiToSwaggerCommand.IsAspNetCore == false &&
                    document.Runtime != Runtime.Debug &&
                    document.Runtime != Runtime.WinX86 &&
                    document.Runtime != Runtime.WinX64)
                {
                    throw new InvalidOperationException("The runtime " + document.Runtime + " in the document must be used " +
                                                        "with ASP.NET Core. Enable /isAspNetCore:true.");
                }
            }

            await document.ExecuteAsync();
            host.WriteMessage("Done.\n");
        }
    }
}
