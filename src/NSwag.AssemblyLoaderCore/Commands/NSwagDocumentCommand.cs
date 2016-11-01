//-----------------------------------------------------------------------
// <copyright file="NSwagDocumentCommand.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.Commands;

namespace NSwag.CodeGeneration.Commands
{
    /// <summary></summary>
    /// <seealso cref="NSwag.Commands.NSwagDocumentCommandBase" />
    public class NSwagDocumentCommand : NSwagDocumentCommandBase
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