//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.Commands.Document;

namespace NSwag.CodeGeneration.Commands.Documents
{
    /// <summary>Creates a new document.</summary>
    public class CreateDocumentCommand : CreateDocumentCommandBase
    {
        /// <summary>Creates a new document in the given path.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The task.</returns>
        protected override async Task CreateDocumentAsync(string filePath)
        {
            var document = new NSwagDocument();
            document.Path = filePath;
            await document.SaveAsync();
        }
    }
}