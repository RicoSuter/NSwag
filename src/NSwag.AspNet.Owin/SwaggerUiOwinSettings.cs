//-----------------------------------------------------------------------
// <copyright file="SwaggerUiOwinSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.AspNet.Owin
{
    /// <summary>The settings for UseSwaggerUi.</summary>
    public class SwaggerUiOwinSettings : SwaggerOwinSettings
    {
        /// <summary>Gets or sets the swagger UI route.</summary>
        public string SwaggerUiRoute { get; set; } = "/swagger";

        /// <summary>Gets or sets the OAuth2 settings.</summary>
        public OAuth2Settings OAuth2 { get; set; }
    }
}