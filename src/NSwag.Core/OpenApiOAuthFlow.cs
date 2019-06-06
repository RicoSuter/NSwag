//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityScheme.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>Configuration for an OAuth flow.</summary>
    public class OpenApiOAuthFlow
    {
        /// <summary>Gets or sets the authorization URL to be used for this flow.</summary>
        [JsonProperty(PropertyName = "authorizationUrl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string AuthorizationUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow.</summary>
        [JsonProperty(PropertyName = "tokenUrl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string TokenUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow.</summary>
        [JsonProperty(PropertyName = "refreshUrl", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string RefreshUrl { get; set; }

        /// <summary>Gets the available scopes for the OAuth2 security scheme.</summary>
        [JsonProperty(PropertyName = "scopes", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();
    }
}