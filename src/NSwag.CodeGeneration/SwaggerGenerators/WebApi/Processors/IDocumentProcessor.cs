//-----------------------------------------------------------------------
// <copyright file="IDocumentProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Post processes a generated <see cref="SwaggerService"/>.</summary>
    public interface IDocumentProcessor
    {
        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="document">The document.</param>
        void Process(SwaggerService document);
    }
}