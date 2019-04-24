//-----------------------------------------------------------------------
// <copyright file="SwaggerMiddleware.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;

namespace NSwag.AspNetCore
{
    /// <summary>The Swagger middleware settings.</summary>
    public class SwaggerDocumentMiddlewareSettings
    {
        /// <summary>Gets the document name (internal identifier, default: v1).</summary>
        /// <remarks>Ignored when <see cref="Path"/> contains '{documentName}' placeholder.</remarks>
        public string DocumentName { get; set; } = "v1";

        /// <summary>Gets or sets the path to serve the OpenAPI/Swagger document (default: '/swagger/{documentName}/swagger.json').</summary>
        /// <remarks>May contain '{documentName}' placeholder to register multiple routes.</remarks>
        public string Path { get; set; } = "/swagger/{documentName}/swagger.json";

        /// <summary>Gets or sets for how long a <see cref="Exception"/> caught during schema generation is cached.</summary>
        public TimeSpan ExceptionCacheTime { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>Gets or sets the Swagger post process action.
        /// Should only be used to transform the document related to the request.
        /// Caution: This action will not be called by the CLI or NSwagStudio 
        /// (use PostProcess in AddSwaggerDocument instead).</summary>
        public Action<SwaggerDocument, HttpRequest> PostProcess { get; set; }
        
        /// <summary>
        /// Should be used in a case when your application is exposed under different URLs, e.g. the application
        /// is accessible from your internal network and behind a reverse-proxy. In this case, NSwag has to generate
        /// a separate JSON document for every access point. As for the callback, the application has to tell
        /// NSwag which swagger JSON document it should return by providing a unique key that matches the required
        /// access point (either it's an internal network HTTP request or an HTTP request from a reverse-proxy).
        /// Hint: In a case of reverse proxy, the key may include of X-Forwarded-Host/X-Forwarded-Proto header values.
        /// <seealso cref="PostProcess"/> 
        /// </summary>
        public Func<HttpRequest, string> CreateDocumentCacheKey { get; set; } = r => r.Host.ToString();
    }
}
