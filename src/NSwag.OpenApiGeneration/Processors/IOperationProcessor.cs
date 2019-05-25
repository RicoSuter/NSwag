//-----------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NSwag.OpenApiGeneration.Processors.Contexts;

namespace NSwag.OpenApiGeneration.Processors
{
    /// <summary>Post processes a generated <see cref="OpenApiOperation"/>.</summary>
    public interface IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        Task<bool> ProcessAsync(OperationProcessorContext context);
    }
}