//-----------------------------------------------------------------------
// <copyright file="ReDocSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if AspNetOwin
namespace NSwag.AspNet.Owin
#else
namespace NSwag.AspNetCore
#endif
{
    /// <summary>The settings for UseReDoc.</summary>
    public class SwaggerReDocSettings : SwaggerUiSettingsBase
    {
        internal override string TransformHtml(string html)
        {
            return html;
        }
    }
}