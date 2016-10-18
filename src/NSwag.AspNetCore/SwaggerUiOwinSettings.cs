//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
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
    /// <summary>The settings for UseSwaggerUi.</summary>
    public class SwaggerUiOwinSettings : SwaggerOwinSettings
    {
        /// <summary>Gets or sets the swagger UI route (must start with '/').</summary>
        public string SwaggerUiRoute { get; set; } = "/swagger";

        /// <summary>Gets or sets a value indicating whether the Swagger specification should be validated.</summary>
        public bool ValidateSpecification { get; set; } = true;

        /// <summary>Gets or sets the Swagger UI OAuth2 client settings.</summary>
        public OAuth2ClientSettings OAuth2Client { get; set; }

        internal string ActualSwaggerUiRoute => SwaggerUiRoute.Substring(MiddlewareBasePath?.Length ?? 0);
    }
}