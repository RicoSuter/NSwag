//-----------------------------------------------------------------------
// <copyright file="ReDocSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Generation;
using System.Collections.Generic;
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

#if AspNetOwin
        internal override string TransformHtml(string html, IOwinRequest request)
#else
        internal override string TransformHtml(string html, HttpRequest request)
#endif
        {
            html = html.Replace("{AdditionalSettings}", GenerateAdditionalSettings(AdditionalSettings));
            html = html.Replace("{CustomStyle}", GetCustomStyleHtml(request));
            html = html.Replace("{CustomScript}", GetCustomScriptHtml(request));
            return html;
        }
    }
}