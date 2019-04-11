//-----------------------------------------------------------------------
// <copyright file="ReDocSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;
#if AspNetOwin
using Microsoft.Owin;

namespace NSwag.AspNet.Owin
#else
using Microsoft.AspNetCore.Http;

namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseReDoc.</summary>
    public class SwaggerReDocSettings<T> : SwaggerUiSettingsBase<T>
        where T : SwaggerGeneratorSettings, new()
    {
#if AspNetOwin
        internal override string TransformHtml(string html, IOwinRequest request)
#else
        internal override string TransformHtml(string html, HttpRequest request)
#endif
        {
            html = html.Replace("{CustomStyle}", GetCustomStyleHtml());
            html = html.Replace("{CustomScript}", GetCustomScriptHtml());
            return html;
        }
    }
}