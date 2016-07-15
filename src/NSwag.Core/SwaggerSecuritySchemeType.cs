//-----------------------------------------------------------------------
// <copyright file="SwaggerSecuritySchemeType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace NSwag
{
    /// <summary></summary>
    public enum SwaggerSecuritySchemeType
    {
        /// <summary>The security scheme is not defined.</summary>
        Undefined,

        /// <summary>Basic authentication.</summary>
        [EnumMember(Value = "basic")]
        Basic,

        /// <summary>API key authentication.</summary>
        [EnumMember(Value = "apiKey")]
        ApiKey,

        /// <summary>OAuth2 authentication.</summary>
        [EnumMember(Value = "oauth2")]
        OAuth2
    }
}