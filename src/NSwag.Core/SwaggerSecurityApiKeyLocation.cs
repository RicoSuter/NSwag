//-----------------------------------------------------------------------
// <copyright file="SwaggerSecurityApiKeyLocation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace NSwag
{
    /// <summary>Specifies the location of the API Key.</summary>
    public enum SwaggerSecurityApiKeyLocation
    {
        /// <summary>The API key kind is not defined.</summary>
        Undefined,

        /// <summary>In a query parameter.</summary>
        [EnumMember(Value = "query")]
        Query,

        /// <summary>In the HTTP header.</summary>
        [EnumMember(Value = "header")]
        Header
    }
}