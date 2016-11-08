//-----------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Post processes a generated <see cref="SwaggerOperation"/>.</summary>
    public interface IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        bool Process(OperationProcessorContext context);
    }
}