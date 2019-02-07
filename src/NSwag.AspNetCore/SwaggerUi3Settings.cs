//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using NSwag.SwaggerGeneration;

#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseSwaggerUi3.</summary>
    public class SwaggerUi3Settings<T> : SwaggerUiSettingsBase<T>
        where T : SwaggerGeneratorSettings, new()
    {
        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        public bool ValidateSpecification { get; set; } = false;

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        /// <summary>Controls how the API listing is displayed. It can be set to 'none' (default), 'list' (shows operations for each resource), or 'full' (fully expanded: shows operations and their details).</summary>
        public string DocExpansion { get; set; } = "none";

        /// <summary>Specifies the operations sorter in Swagger UI 3.</summary>
        public string OperationsSorter { get; set; } = "none";

        /// <summary>The default expansion depth for models (set to -1 completely hide the models) in Swagger UI 3.</summary>
        public int DefaultModelsExpandDepth { get; set; } = 1;

        /// <summary>The default expansion depth for the model on the model-example section in Swagger UI 3.</summary>
        public int DefaultModelExpandDepth { get; set; } = 1;

        /// <summary>Specifies the tags sorter in Swagger UI 3</summary>
        public string TagsSorter { get; set; } = "none";

        /// <summary>Specifies whether the "Try it out" option is enabled in Swagger UI 3.</summary>
        public bool EnableTryItOut { get; set; } = true;

        /// <summary>Gets or sets the server URL.</summary>
        public string ServerUrl { get; set; } = "";

        /// <summary>Gets or sets the Swagger URL routes (must start with '/', hides SwaggerRoute).</summary>
        public ICollection<SwaggerUi3Route> SwaggerRoutes { get; } = new List<SwaggerUi3Route>();

        internal override string ActualSwaggerDocumentPath => SwaggerRoutes.Any() ? "" : base.ActualSwaggerDocumentPath;

#if AspNetOwin
        internal override string TransformHtml(string html, IOwinRequest request)
#else
        internal override string TransformHtml(string html, HttpRequest request)
#endif
        {
            var oauth2Settings = OAuth2Client ?? new OAuth2ClientSettings();
            foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
            {
                var value = property.GetValue(oauth2Settings);
                html = html.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
            }

            html = html.Replace("{Urls}", !SwaggerRoutes.Any() ?
                "undefined" :
                JsonConvert.SerializeObject(
                    SwaggerRoutes.Select(r => new SwaggerUi3Route(r.Name, TransformToExternalPath(r.Url.Substring(MiddlewareBasePath?.Length ?? 0), request)))
                ));

            html = html.Replace("{ValidatorUrl}", ValidateSpecification ? "undefined" : "null");
            html = html.Replace("{DocExpansion}", DocExpansion);
            html = html.Replace("{OperationsSorter}", OperationsSorter);
            html = html.Replace("{DefaultModelsExpandDepth}", DefaultModelsExpandDepth.ToString());
            html = html.Replace("{DefaultModelExpandDepth}", DefaultModelExpandDepth.ToString());
            html = html.Replace("{TagsSorter}", TagsSorter);
            html = html.Replace("{EnableTryItOut}", EnableTryItOut.ToString().ToLower());
            html = html.Replace("{RedirectUrl}", string.IsNullOrEmpty(ServerUrl) ?
                "window.location.origin + \"" + TransformToExternalPath(Path, request) + "/oauth2-redirect.html\"" :
                "\"" + ServerUrl + TransformToExternalPath(Path, request) + "/oauth2-redirect.html\"");
            html = html.Replace("{StylesheetUri}", CustomStylesheetUri?.ToString() ?? "");
            html = html.Replace("{ScriptUri}", CustomJavaScriptUri?.ToString() ?? "");

            return html;
        }
    }

    /// <summary>Specifies a route in the Swagger dropdown.</summary>
    public class SwaggerUi3Route
    {
        public SwaggerUi3Route(string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            Name = name;
            Url = url;
        }

        [JsonProperty("url")]
        public string Url { get; internal set; }

        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}