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

#pragma warning disable 1591

namespace NSwag.Commands.Document
{
    [Command(Name = "new", Description = "Creates a new nswag.json file in the current directory.")]
    public abstract class CreateDocumentCommandBase : IConsoleCommand
    {
        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (!DynamicApis.FileExists("nswag.json"))
            {
                await CreateDocumentAsync("nswag.json");
                host.WriteMessage("nswag.json file created.");
            }
            else
                host.WriteMessage("nswag.json already exists.");

            return null; 
        }

        /// <summary>Creates a new document in the given path.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The task.</returns>
        protected abstract Task CreateDocumentAsync(string filePath);
    }
}