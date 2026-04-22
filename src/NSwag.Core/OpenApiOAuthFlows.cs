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
    /// <summary>Configuration information for the supported flow types.</summary>
    public class OpenApiOAuthFlows
    {
        /// <summary>Gets or sets the configuration for the OAuth Implicit Code flow.</summary>
        [JsonPropertyName("implicit")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiOAuthFlow Implicit { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Resource Owner Password flow.</summary>
        [JsonPropertyName("password")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiOAuthFlow Password { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Client Credentials flow.</summary>
        [JsonPropertyName("clientCredentials")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiOAuthFlow ClientCredentials { get; set; }

        /// <summary>Gets or sets the configuration for the OAuth Authorization Code flow.</summary>
        [JsonPropertyName("authorizationCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiOAuthFlow AuthorizationCode { get; set; }
    }
}