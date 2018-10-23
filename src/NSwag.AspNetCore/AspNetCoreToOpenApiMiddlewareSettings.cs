//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToOpenApiMiddlewareSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore.Middlewares;

namespace NSwag.AspNetCore
{
    /// <summary>
    /// The settings for <see cref="OpenApiExtensions.UseOpenApi"/> and <see cref="AspNetCoreToOpenApiMiddleware"/>.
    /// </summary>
    /// <remarks>
    /// Reviewers: Is it useful to have a post-processing step i.e. PostProcess property here that all documents share?
    /// </remarks>
    public class AspNetCoreToOpenApiMiddlewareSettings
    {
        /// <summary>
        /// Gets or sets the Swagger URL route. Must start with '/' and should contain a '{documentName}' route
        /// parameter.
        /// </summary>
        public string SwaggerRoute { get; set; } = "/swagger/{documentName}/swagger.json";

        /// <summary>
        /// Gets or sets for how long an <see cref="Exception"/> caught during schema generation is cached.
        /// </summary>
        public TimeSpan ExceptionCacheTime { get; set; } = TimeSpan.FromSeconds(10);
    }
}
