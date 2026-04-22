//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityScheme.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace NSwag
{
    /// <summary>Configuration for an OAuth flow.</summary>
    public class OpenApiOAuthFlow
    {
        /// <summary>Gets or sets the authorization URL to be used for this flow.</summary>
        [JsonPropertyName("authorizationUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string AuthorizationUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow.</summary>
        [JsonPropertyName("tokenUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string TokenUrl { get; set; }

        /// <summary>Gets or sets the token URL to be used for this flow.</summary>
        [JsonPropertyName("refreshUrl")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string RefreshUrl { get; set; }

        /// <summary>Gets the available scopes for the OAuth2 security scheme.</summary>
        [JsonPropertyName("scopes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, string> Scopes { get; set; } = new Dictionary<string, string>();
    }
}