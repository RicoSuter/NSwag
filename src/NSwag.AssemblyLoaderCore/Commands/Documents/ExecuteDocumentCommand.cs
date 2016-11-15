//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.Commands;
using NSwag.Commands.Document;

namespace NSwag.CodeGeneration.Commands.Documents
{
    /// <summary>Executes a document.</summary>
    public class ExecuteDocumentCommand : ExecuteDocumentCommandBase
    {
        /// <summary>Loads an existing NSwagDocument.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The document.</returns>
        protected override async Task<NSwagDocumentBase> LoadDocumentAsync(string filePath)
        {
            return await NSwagDocument.LoadAsync(filePath);
        }
    }
}