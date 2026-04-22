//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityApiKeyLocation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using NSwag.Converters;

namespace NSwag
{
    /// <summary>Specifies the location of the API Key.</summary>
    [JsonConverter(typeof(EnumMemberStringEnumConverter<OpenApiSecurityApiKeyLocation>))]
    public enum OpenApiSecurityApiKeyLocation
    {
        /// <summary>The API key kind is not defined.</summary>
        Undefined,

        /// <summary>In a query parameter.</summary>
        [EnumMember(Value = "query")]
        Query,

        /// <summary>In the HTTP header.</summary>
        [EnumMember(Value = "header")]
        Header,

        /// <summary>In the HTTP cookie (OpenAPI only).</summary>
        [EnumMember(Value = "cookie")]
        Cookie
    }
}
