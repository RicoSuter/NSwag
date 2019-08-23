//-----------------------------------------------------------------------
// <copyright file="ISwaggerDocumentProvider.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NSwag.Generation
{
    /// <summary>The <see cref="IOpenApiDocumentGenerator"/> interface.</summary>
    public interface IOpenApiDocumentGenerator
    {
        /// <summary>Generates the specified document.</summary>
        /// <param name="documentName">The document name.</param>
        /// <returns>The document.</returns>
        Task<OpenApiDocument> GenerateAsync(string documentName);
    }
}
