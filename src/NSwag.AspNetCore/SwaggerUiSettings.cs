﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NSwag.Generation;

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
        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        public bool ValidateSpecification { get; set; } = true;

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        /// <summary>Gets or sets the Swagger UI supported submit methods. An array of of the HTTP operations that will have the 'Try it out!' option.</summary>
        public string[] SupportedSubmitMethods { get; set; } = new[] { "get", "post", "put", "delete", "patch" };

        /// <summary>Controls how the API listing is displayed. It can be set to 'none' (default), 'list' (shows operations for each resource), or 'full' (fully expanded: shows operations and their details).</summary>
        public string DocExpansion { get; set; } = "none";

        /// <summary>Enables a graphical view for editing complex bodies. Defaults to false.</summary>
        public bool UseJsonEditor { get; set; } = false;

        /// <summary>Controls how models are shown when the API is first rendered. (The user can always switch the rendering for a given model by clicking the 'Model' and 'Model Schema' links.) It can be set to 'model' or 'schema', and the default is 'schema'.</summary>
        public string DefaultModelRendering { get; set; } = "schema";

        /// <summary>Whether or not to show the headers that were sent when making a request via the 'Try it out!' option. Defaults to false.</summary>
        public bool ShowRequestHeaders { get; set; } = false;

#if AspNetOwin
        internal override string TransformHtml(string html, IOwinRequest request)
#else
        internal override string TransformHtml(string html, HttpRequest request)
#endif
        {
            var htmlBuilder = new StringBuilder(html);
            var oauth2Settings = OAuth2Client ?? new OAuth2ClientSettings();
            foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
            {
                var value = property.GetValue(oauth2Settings);
                htmlBuilder.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
            }

            htmlBuilder.Replace("{ValidatorUrl}", ValidateSpecification ? "undefined" : "null")
                .Replace("{DocExpansion}", DocExpansion)
                .Replace("{SupportedSubmitMethods}", JsonConvert.SerializeObject(SupportedSubmitMethods ?? new string[] { }))
                .Replace("{UseJsonEditor}", UseJsonEditor ? "true" : "false")
                .Replace("{DefaultModelRendering}", DefaultModelRendering)
                .Replace("{ShowRequestHeaders}", ShowRequestHeaders ? "true" : "false")
                .Replace("{CustomStyle}", GetCustomStyleHtml(request))
                .Replace("{CustomScript}", GetCustomScriptHtml(request));

            return htmlBuilder.ToString();
        }
    }
}