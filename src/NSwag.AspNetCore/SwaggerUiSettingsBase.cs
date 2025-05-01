//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Generation;
using System;
using System.Collections.Generic;
using NJsonSchema;
using System.Globalization;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The base settings for all Swagger UIs.</summary>
#if AspNetOwin
    public abstract class SwaggerUiSettingsBase<T> : SwaggerSettings<T>
        where T : OpenApiDocumentGeneratorSettings, new()
#else
    public abstract class SwaggerUiSettingsBase : SwaggerSettings
#endif
    {
        /// <summary>Initializes a new instance of the class.</summary>
        public SwaggerUiSettingsBase()
        {
            TransformToExternalPath = (internalUiRoute, request) =>
                '/' + (request.GetBasePath()?.TrimEnd('/') + '/' + internalUiRoute?.TrimStart('/')).Trim('/');
        }

        /// <summary>Gets or sets the internal swagger UI route (must start with '/').</summary>
        public string Path { get; set; } = "/swagger";

#pragma warning disable 618
        internal string ActualSwaggerUiPath => Path.Substring(MiddlewareBasePath?.Length ?? 0);
#pragma warning restore 618

        /// <summary>Gets or sets custom inline styling to inject into the index.html</summary>
        public string CustomInlineStyles { get; set; }

        /// <summary>Gets or sets a URI to load a custom CSS Stylesheet into the index.html</summary>
        public string CustomStylesheetPath { get; set; }

        /// <summary>Gets or sets a URI to load a custom JavaScript file into the index.html.</summary>
        public string CustomJavaScriptPath { get; set; }

        /// <summary>Gets the additional Swagger UI plugins settings. Add key-value-paris that represent the plugin object name as key and plugin sript path as value.</summary>
        public Dictionary<string,string> AdditionalPlugins { get; } = new Dictionary<string, string>();

        /// <summary>Gets or sets a flag that indicates to use or not type="module" in a custom script tag (default: false).</summary>
        public bool UseModuleTypeForCustomJavaScript { get; set; }

        /// <summary>Gets or sets the external route base path (must start with '/', default: null = use SwaggerUiRoute).</summary>
#if AspNetOwin
        public Func<string, IOwinRequest, string> TransformToExternalPath { get; set; }

        internal abstract Task<string> TransformHtmlAsync(string html, IOwinRequest request, CancellationToken cancellationToken);
#else
        public Func<string, HttpRequest, string> TransformToExternalPath { get; set; }

        internal abstract Task<string> TransformHtmlAsync(string html, HttpRequest request, CancellationToken cancellationToken);
#endif

        /// <summary>
        /// Gets an HTML snippet for including custom StyleSheet in swagger UI.
        /// </summary>
#if AspNetOwin
        protected string GetCustomStyleHtml(IOwinRequest request)
#else
        protected string GetCustomStyleHtml(HttpRequest request)
#endif
        {
            var html = new StringBuilder();

            if (!string.IsNullOrEmpty(CustomStylesheetPath))
            {
                var uriString = System.Net.WebUtility.HtmlEncode(TransformToExternalPath(CustomStylesheetPath, request));
                html.AppendLine($"<link rel=\"stylesheet\" href=\"{uriString}\">");
            }
            else if (!string.IsNullOrEmpty(CustomInlineStyles))
            {
                html.AppendLine($"<style type=\"text/css\">{CustomInlineStyles}</style>");
            }

            return html.ToString();
        }

        /// <summary>
        /// Gets an HTML snippet for including custom JavaScript in swagger UI.
        /// </summary>
#if AspNetOwin
        protected string GetCustomScriptHtml(string scriptPath, IOwinRequest request)
#else
        protected string GetCustomScriptHtml(string scriptPath, HttpRequest request)
#endif
        {
            var scriptType = string.Empty;
            if (UseModuleTypeForCustomJavaScript)
            {
                scriptType = "type=\"module\"";
            }

            var uriString = System.Net.WebUtility.HtmlEncode(TransformToExternalPath(scriptPath, request));
            return $"<script {scriptType} src=\"{uriString}\"></script>";
        }

        /// <summary>
        /// Gets an HTML snippet for including custom JavaScript in swagger UI.
        /// </summary>
#if AspNetOwin
        protected string GetCustomScripts(string[] scriptPaths, IOwinRequest request)
#else
        protected string GetCustomScripts(string[] scriptPaths, HttpRequest request)
#endif
        {
            if ((scriptPaths == null ) || (scriptPaths.Count() == 0))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();    
            foreach (var path in scriptPaths)
            {
                var scriptTag = GetCustomScriptHtml(path, request);
                builder.Append(scriptTag + "\n          ");
            }

            return builder.ToString();
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

        /// <summary>
        /// Generates the JavaScript plugins object to inset into the html.
        /// </summary>
        protected string GeneratePluginsList(string[] pluginsList)
        {
            var builder = new StringBuilder();
            foreach (var plugin in pluginsList)
            {
                builder.Append(",\n   ");
                builder.Append(plugin);
            }
            return builder.ToString();
        }
    }
}
