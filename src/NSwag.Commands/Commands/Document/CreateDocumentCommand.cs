//-----------------------------------------------------------------------
// <copyright file="CreateDocumentCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using NJsonSchema.Infrastructure;
using NSwag.Commands.CodeGeneration;

#pragma warning disable 1591

namespace NSwag.Commands.Document
{
    [Command(Name = "new", Description = "Creates a new nswag.json file in the current directory.")]
    public class CreateDocumentCommand : IConsoleCommand
    {
        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (await DynamicApis.FileExistsAsync("nswag.json").ConfigureAwait(false) == false)
            {
                await CreateDocumentAsync("nswag.json");
                host.WriteMessage("nswag.json file created.");
            }
            else
                host.WriteMessage("nswag.json already exists.");

            return null; 
        }

        private async Task CreateDocumentAsync(string filePath)
        {
            var document = new NSwagDocument();
            document.Path = filePath;

            document.CodeGenerators.SwaggerToCSharpControllerCommand = new SwaggerToCSharpControllerCommand();
            document.CodeGenerators.SwaggerToCSharpClientCommand = new SwaggerToCSharpClientCommand();
            document.CodeGenerators.SwaggerToTypeScriptClientCommand = new SwaggerToTypeScriptClientCommand();

            await document.SaveAsync();
        }
    }
}