//-----------------------------------------------------------------------
// <copyright file="SwaggerSecuritySchemeType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary></summary>
    public enum SwaggerSecuritySchemeType
    {
        /// <summary>The security scheme is not defined.</summary>
        Undefined,

        /// <summary>Basic authentication.</summary>
        [JsonProperty("basic")]
        Basic,

        /// <summary>API key authentication.</summary>
        [JsonProperty("apiKey")]
        ApiKey,

        /// <summary>OAuth2 authentication.</summary>
        [JsonProperty("oauth2")]
        OAuth2
    }
}