//-----------------------------------------------------------------------
// <copyright file="ISwaggerDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NSwag.SwaggerGeneration
{
    /// <summary>The SwaggerDocumentProvider inteface.</summary>
    public interface ISwaggerDocumentProvider
    {
        /// <summary>Generates the specified document.</summary>
        /// <param name="documentName">The document name.</param>
        /// <returns>The document.</returns>
        Task<SwaggerDocument> GenerateAsync(string documentName);
    }
}
