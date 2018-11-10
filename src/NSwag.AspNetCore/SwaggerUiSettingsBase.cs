//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;
using System;
#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The base settings for all Swagger UIs.</summary>
    public abstract class SwaggerUiSettingsBase<T> : SwaggerSettings<T>
        where T : SwaggerGeneratorSettings, new()
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerUiSettingsBase{T}"/> class.</summary>
        public SwaggerUiSettingsBase()
        {
            TransformToExternalPath = (internalUiRoute, request) => internalUiRoute;
        }

        /// <summary>Gets or sets the internal swagger UI route (must start with '/').</summary>
        public string Path { get; set; } = "/swagger";

        internal string ActualSwaggerUiPath => Path.Substring(MiddlewareBasePath?.Length ?? 0);

        /// <summary>Gets or sets the external route base path (must start with '/', default: null = use SwaggerUiRoute).</summary>
#if AspNetOwin
        public Func<string, IOwinRequest, string> TransformToExternalPath { get; set; }

        internal abstract string TransformHtml(string html, IOwinRequest request);
#else
        public Func<string, HttpRequest, string> TransformToExternalPath { get; set; }

        internal abstract string TransformHtml(string html, HttpRequest request);
#endif
    }
}