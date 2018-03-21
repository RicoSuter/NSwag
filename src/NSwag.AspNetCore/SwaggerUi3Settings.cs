//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Reflection;
using Newtonsoft.Json;
using NSwag.SwaggerGeneration;

#if AspNetOwin
namespace NSwag.AspNet.Owin
#else
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

        /// <summary>Gets or sets the server URL.</summary>
        public string ServerUrl { get; set; } = "";

        internal override string TransformHtml(string html)
        {
            var oauth2Settings = OAuth2Client ?? new OAuth2ClientSettings();
            foreach (var property in oauth2Settings.GetType().GetRuntimeProperties())
            {
                var value = property.GetValue(oauth2Settings);
                html = html.Replace("{" + property.Name + "}", value is IDictionary ? JsonConvert.SerializeObject(value) : value?.ToString() ?? "");
            }

            html = html.Replace("{ValidatorUrl}", ValidateSpecification ? "undefined" : "null");
            html = html.Replace("{DocExpansion}", DocExpansion);
            html = html.Replace("{RedirectUrl}", string.IsNullOrEmpty(ServerUrl) ?
                "window.location.origin + \"" + SwaggerUiRoute + "/oauth2-redirect.html\"" :
                "\"" + ServerUrl + SwaggerUiRoute + "/oauth2-redirect.html\"");

            return html;
        }
    }
}