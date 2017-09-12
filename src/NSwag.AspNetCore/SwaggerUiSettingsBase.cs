//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
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
    /// <summary>The base settings for all Swagger UIs.</summary>
    public class SwaggerUiSettingsBase : SwaggerSettings
    {
        /// <summary>Gets or sets the swagger UI route (must start with '/').</summary>
        public string SwaggerUiRoute { get; set; } = "/swagger";

        internal string ActualSwaggerUiRoute => SwaggerUiRoute.Substring(MiddlewareBasePath?.Length ?? 0);
    }
}