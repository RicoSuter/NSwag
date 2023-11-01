//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NSwag.Generation;
using NJsonSchema;
using System.Threading;
using System.Threading.Tasks;

#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseSwaggerUi.</summary>
#if AspNetOwin
    public class SwaggerUiSettings<T> : SwaggerUiSettingsBase<T>
        where T : OpenApiDocumentGeneratorSettings, new()
#else
    public class SwaggerUiSettings : SwaggerUiSettingsBase
#endif
    {
        /// <summary>Initializes a new instance of the class.</summary>
        public SwaggerUiSettings()
        {
            DocExpansion = "none";
            OperationsSorter = "none";
            DefaultModelsExpandDepth = 1;
            DefaultModelExpandDepth = 1;
            TagsSorter = "none";
        }

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        /// <summary>Gets or sets the server URL.</summary>
        public string ServerUrl { get; set; } = "";

        /// <summary>Specifies whether the "Try it out" option is enabled in Swagger UI 3.</summary>
        public bool EnableTryItOut { get; set; } = true;

        /// <summary>
        /// Gets or sets a title for the Swagger UI page.
        /// </summary>
        public string DocumentTitle { get; set; } = "Swagger UI";

        /// <summary>
        /// Gets or sets additional content to place in the head of the Swagger UI page.
        /// </summary>
        public string CustomHeadContent { get; set; } = "";

        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        public bool ValidateSpecification { get; set; } = false;

        /// <summary>Gets the additional Swagger UI 3 settings.</summary>
        public IDictionary<string, object> AdditionalSettings { get; } = new Dictionary<string, object>();

        /// <summary>Controls how the API listing is displayed. It can be set to 'none' (default), 'list' (shows operations for each resource), or 'full' (fully expanded: shows operations and their details).</summary>
        public string DocExpansion
        {
            get => (string)AdditionalSettings["docExpansion"];
            set => AdditionalSettings["docExpansion"] = value;
        }

        /// <summary>Specifies the operations sorter in Swagger UI 3.</summary>
        public string OperationsSorter
        {
            get => (string)AdditionalSettings["operationsSorter"];
            set => AdditionalSettings["operationsSorter"] = value;
        }

        /// <summary>The default expansion depth for models (set to -1 completely hide the models) in Swagger UI 3.</summary>
        public int DefaultModelsExpandDepth
        {
            get => (int)AdditionalSettings["defaultModelsExpandDepth"];
            set => AdditionalSettings["defaultModelsExpandDepth"] = value;
        }

        /// <summary>The default expansion depth for the model on the model-example section in Swagger UI 3.</summary>
        public int DefaultModelExpandDepth
        {
            get => (int)AdditionalSettings["defaultModelExpandDepth"];
            set => AdditionalSettings["defaultModelExpandDepth"] = value;
        }

        /// <summary>Specifies the tags sorter in Swagger UI 3</summary>
        public string TagsSorter
        {
            get => (string)AdditionalSettings["tagsSorter"];
            set => AdditionalSettings["tagsSorter"] = value;
        }

        /// <summary>Specifies whether to persist authorization data in Swagger UI 3.</summary>
        public bool PersistAuthorization
        {
            get => (bool)AdditionalSettings["persistAuthorization"];
            set => AdditionalSettings["persistAuthorization"] = value;
        }

        /// <summary>Gets a value indicating whether to send credentials from the Swagger UI 3 to the backend.</summary>
        public bool WithCredentials
        {
            get => (bool)AdditionalSettings["withCredentials"];
            set => AdditionalSettings["withCredentials"] = value;
        }

        /// <summary>Gets or sets the Swagger URL routes (must start with '/', hides SwaggerRoute).</summary>
        public ICollection<SwaggerUi3Route> SwaggerRoutes { get; } = new List<SwaggerUi3Route>();

        /// <summary>Gets or sets the Swagger URL routes factory (SwaggerRoutes is ignored when set).</summary>
#if AspNetOwin
        public Func<IOwinRequest, CancellationToken, Task<IEnumerable<SwaggerUi3Route>>> SwaggerRoutesFactory { get; set; }
#else
        public Func<HttpRequest, CancellationToken, Task<IEnumerable<SwaggerUi3Route>>> SwaggerRoutesFactory { get; set; }
#endif

        internal override string ActualSwaggerDocumentPath => SwaggerRoutes.Any() ? "" : base.ActualSwaggerDocumentPath;

#if AspNetOwin
        internal override async Task<string> TransformHtmlAsync(string html, IOwinRequest request, CancellationToken cancellationToken)
#else
        internal override async Task<string> TransformHtmlAsync(string html, HttpRequest request, CancellationToken cancellationToken)
#endif
        {
            var htmlBuilder = new StringBuilder(html);
            var oauth2Settings = OAuth2Client ?? new OAuth2ClientSettings();
            foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
            {
                var value = property.GetValue(oauth2Settings);
                if (value is ICollection collection)
                {
                    htmlBuilder.Replace("{" + property.Name + "}", JsonConvert.SerializeObject(collection));
                }
                else if (value is bool boolean)
                {
                    htmlBuilder.Replace("{" + property.Name + "}", boolean.ToString().ToLowerInvariant());
                }
                else
                {
                    htmlBuilder.Replace("{" + property.Name + "}", value?.ToString() ?? "");
                }
            }

            var swaggerRoutes = SwaggerRoutesFactory != null ? 
                (await SwaggerRoutesFactory(request, cancellationToken)).ToList() : 
                SwaggerRoutes;

            htmlBuilder.Replace("{Urls}", !swaggerRoutes.Any()
                ? "undefined"
                : JsonConvert.SerializeObject(
#pragma warning disable 618
                    swaggerRoutes.Select(r => new SwaggerUi3Route(r.Name,
                        TransformToExternalPath(r.Url.Substring(MiddlewareBasePath?.Length ?? 0), request)))
#pragma warning restore 618
                ));

            htmlBuilder.Replace("{ValidatorUrl}", ValidateSpecification ? "undefined" : "null")
                .Replace("{AdditionalSettings}", GenerateAdditionalSettings(AdditionalSettings))
                .Replace("{EnableTryItOut}", EnableTryItOut.ToString().ToLower())
                .Replace("{RedirectUrl}",
                    string.IsNullOrEmpty(ServerUrl)
                        ? "window.location.origin + \"" + TransformToExternalPath(Path, request) +
                          "/oauth2-redirect.html\""
                        : "\"" + ServerUrl + TransformToExternalPath(Path, request) + "/oauth2-redirect.html\"")
                .Replace("{CustomStyle}", GetCustomStyleHtml(request))
                .Replace("{CustomScript}", GetCustomScriptHtml(request))
                .Replace("{CustomHeadContent}", CustomHeadContent)
                .Replace("{DocumentTitle}", DocumentTitle);

            return htmlBuilder.ToString();
        }
    }

    /// <summary>Specifies a route in the Swagger dropdown.</summary>
    public class SwaggerUi3Route
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerUi3Route"/> class.</summary>
        public SwaggerUi3Route(string name, string url)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            Name = name;
            Url = url;
        }

        /// <summary>Gets the route URL.</summary>
        [JsonProperty("url")]
        public string Url { get; internal set; }

        /// <summary>Gets the route name.</summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }
    }
}