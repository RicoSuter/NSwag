//-----------------------------------------------------------------------
// <copyright file="ISwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace NSwag.SwaggerGeneration
{
    /// <summary>Interface for a service provider based Swagger document generator.</summary>
    public interface ISwaggerGenerator
    {
        /// <summary>Generates the <see cref="SwaggerDocument"/>.</summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>The document</returns>
        Task<SwaggerDocument> GenerateAsync(object serviceProvider);
    }
}
