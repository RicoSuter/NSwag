//-----------------------------------------------------------------------
// <copyright file="ReDocSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Generation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseReDoc.</summary>
#if AspNetOwin
    public class ReDocSettings<T> : SwaggerUiSettingsBase<T>
        where T : OpenApiDocumentGeneratorSettings, new()
#else
    public class ReDocSettings : SwaggerUiSettingsBase
#endif
    {
        /// <summary>Gets the additional ReDoc settings.</summary>
        public IDictionary<string, object> AdditionalSettings { get; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets a title for the ReDoc page.
        /// </summary>
        public string DocumentTitle { get; set; } = "ReDoc";

#if AspNetOwin
        internal override Task<string> TransformHtmlAsync(string html, IOwinRequest request, CancellationToken cancellationToken)
#else
        internal override Task<string> TransformHtmlAsync(string html, HttpRequest request, CancellationToken cancellationToken)
#endif
        {
            html = html.Replace("{AdditionalSettings}", GenerateAdditionalSettings(AdditionalSettings));
            html = html.Replace("{CustomStyle}", GetCustomStyleHtml(request));
            html = html.Replace("{CustomScript}", GetCustomScriptHtml(request));
            html = html.Replace("{DocumentTitle}", DocumentTitle);
            return Task.FromResult(html);
        }
    }
}