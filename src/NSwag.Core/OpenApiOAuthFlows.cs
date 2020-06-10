//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityScheme.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>Configuration information for the supported flow types.</summary>
    public class OpenApiOAuthFlows
    {
        /// <summary>Gets or sets the configuration for the OAuth Implicit Code flow.</summary>
        [JsonProperty(PropertyName = "implicit", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public OpenApiOAuthFlow Implicit { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Resource Owner Password flow.</summary>
        [JsonProperty(PropertyName = "password", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public OpenApiOAuthFlow Password { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Client Credentials flow.</summary>
        [JsonProperty(PropertyName = "clientCredentials", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public OpenApiOAuthFlow ClientCredentials { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Authorization Code flow.</summary>
        [JsonProperty(PropertyName = "authorizationCode", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public OpenApiOAuthFlow AuthorizationCode { get; set; }
    }
}