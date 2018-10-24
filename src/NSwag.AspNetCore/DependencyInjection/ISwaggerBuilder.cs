//-----------------------------------------------------------------------
// <copyright file="ISwaggerBuilder.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// An interface for configuring Swagger and Open API documents.
    /// </summary>
    public interface ISwaggerBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/>.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
