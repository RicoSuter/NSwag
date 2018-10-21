//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;

namespace NSwag.AspNetCore.Middlewares
{
    /// <summary>The Swagger middleware settings.</summary>
    public class SwaggerMiddlewareSettings
    {
        /// <summary>Gets or sets the path to serve the OpenAPI/Swagger document.</summary>
        public string Path { get; set; } = "swagger/v1/swagger.json";

        /// <summary>Gets or sets for how long a <see cref="Exception"/> caught during schema generation is cached.</summary>
        public TimeSpan ExceptionCacheTime { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>Gets or sets the Swagger post process action.</summary>
        public Action<HttpRequest, SwaggerDocument> PostProcess { get; set; }

        /// <summary>Gets or sets the middleware base path (must start with '/').</summary>
        public string MiddlewareBasePath { get; set; }
    }
}
