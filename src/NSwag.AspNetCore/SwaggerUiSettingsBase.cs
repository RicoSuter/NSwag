//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;
using System;
using System.Collections.Generic;
using NJsonSchema;
using System.Globalization;
using Newtonsoft.Json;
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

        /// <summary>Gets or sets a URI to load a custom CSS Stylesheet into the index.html</summary>
        public Uri CustomStylesheetUri { get; set; }

        /// <summary>Gets or sets a URI to load a custom JavaScript file into the index.html.</summary>
        public Uri CustomJavaScriptUri { get; set; }

        /// <summary>Gets or sets the external route base path (must start with '/', default: null = use SwaggerUiRoute).</summary>
#if AspNetOwin
        public Func<string, IOwinRequest, string> TransformToExternalPath { get; set; }

        internal abstract string TransformHtml(string html, IOwinRequest request);
#else
        public Func<string, HttpRequest, string> TransformToExternalPath { get; set; }

        internal abstract string TransformHtml(string html, HttpRequest request);
#endif

        /// <summary>
        /// Gets an HTML snippet for including custom StyleSheet in swagger UI.
        /// </summary>
        protected string GetCustomStyleHtml()
        {
            if (CustomStylesheetUri == null)
            {
                return string.Empty;
            }

            var uriString = System.Net.WebUtility.HtmlEncode(CustomStylesheetUri.OriginalString);

            return $"<link rel=\"stylesheet\" href=\"{uriString}\">";
        }

        /// <summary>
        /// Gets an HTML snippet for including custom JavaScript in swagger UI.
        /// </summary>
        protected string GetCustomScriptHtml()
        {
            if (CustomJavaScriptUri == null)
            {
                return string.Empty;
            }
            
            var uriString = System.Net.WebUtility.HtmlEncode(CustomJavaScriptUri.OriginalString);

            return $"<script src=\"{uriString}\"></script>";
        }

        /// <summary>Generates the additional objects JavaScript code.</summary>
        /// <param name="additionalSettings">The additional settings.</param>
        /// <returns>The code.</returns>
        protected string GenerateAdditionalSettings(IDictionary<string, object> additionalSettings)
        {
            var code = "";
            foreach (var pair in additionalSettings)
            {
                code += pair.Key + ": " + JsonConvert.SerializeObject(pair.Value) + ", \n    ";
            }

            return code;
        }
    }
}
