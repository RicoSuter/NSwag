//-----------------------------------------------------------------------
// <copyright file="SwaggerUiSettingsBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration;

#if AspNetOwin
namespace NSwag.AspNet.Owin
#else
namespace NSwag.AspNetCore
#endif
{
    /// <summary>The base settings for all Swagger UIs.</summary>
    public abstract class SwaggerUiSettingsBase<T> : SwaggerSettings<T>
        where T : SwaggerGeneratorSettings, new()
    {
        /// <summary>Gets or sets the swagger UI route (must start with '/').</summary>
        public string SwaggerUiRoute { get; set; } = "/swagger";

        internal string ActualSwaggerUiRoute => SwaggerUiRoute.Substring(MiddlewareBasePath?.Length ?? 0);

        internal abstract string TransformHtml(string html);
    }
}