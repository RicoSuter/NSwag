//-----------------------------------------------------------------------
// <copyright file="IOpenApiBuilder.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;

namespace NSwag.AspNetCore.DependencyInjection
{
    /// <summary>
    /// An interface for configuring Open Api documents.
    /// </summary>
    public interface IOpenApiBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/>.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
