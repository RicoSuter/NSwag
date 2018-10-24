//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddlewareOptions.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.ApiDescription;
using NSwag.AspNetCore.Middlewares;

namespace NSwag.AspNetCore
{
    /// <summary>
    /// The settings for <see cref="SwaggerApplicationBuilderExtensions.UseSwagger(IApplicationBuilder)"/> and
    /// <see cref="SwaggerMiddleware"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Reviewers: Could add a <c>DocumentName</c> property in support of a middleware addition that supports a single
    /// document. Does not seem necessary to me.
    /// </para>
    /// </remarks>
    public class SwaggerMiddlewareOptions
    {
        /// <summary>
        /// Gets or sets the Swagger URL route. Must start with '/' and should contain a '{documentName}' route
        /// parameter.
        /// </summary>
        /// <remarks>
        /// Reviewers: It's actually not legal for an MVC route to begin with a slash. But, I left this as-is for
        /// consistency with so-called <c>SwaggerRoute</c> and <c>SwaggerUiRoute</c> values elsewhere.
        /// </remarks>
        public string SwaggerRoute { get; set; } = "/swagger/{documentName}/swagger.json";

        /// <summary>
        /// Gets or sets for how long an <see cref="Exception"/> caught during schema generation is cached.
        /// </summary>
        public TimeSpan ExceptionCacheTime { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Gets or sets a post-process action that will be applied to all documents this middleware supports.
        /// </summary>
        /// <remarks>
        /// Reviewers: One issue with placing this here instead of in <see cref="SwaggerDocumentSettings"/> is the
        /// post-processing cannot be done for the <see cref="SwaggerDocumentProvider"/>, potentially messing up what
        /// <see cref="IDocumentProvider.GenerateAsync"/> writes.
        /// </remarks>
        public Action<SwaggerDocument> PostProcess { get; set; }
    }
}
