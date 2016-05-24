//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationMethod.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag
{
    /// <summary>Enumeration of the available HTTP methods. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SwaggerOperationMethod
    {
        /// <summary>An undefined method.</summary>
        [EnumMember(Value = "undefined")]
        Undefined,

        /// <summary>The HTTP GET method. </summary>
        [EnumMember(Value = "get")]
        Get,

        /// <summary>The HTTP POST method. </summary>
        [EnumMember(Value = "post")]
        Post,

        /// <summary>The HTTP PUT method. </summary>
        [EnumMember(Value = "put")]
        Put,

        /// <summary>The HTTP DELETE method. </summary>
        [EnumMember(Value = "delete")]
        Delete,

        /// <summary>The HTTP OPTIONS method. </summary>
        [EnumMember(Value = "options")]
        Options,

        /// <summary>The HTTP HEAD method. </summary>
        [EnumMember(Value = "head")]
        Head,

        /// <summary>The HTTP PATCH method. </summary>
        [EnumMember(Value = "patch")]
        Patch
    }
}