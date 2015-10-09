//-----------------------------------------------------------------------
// <copyright file="SwaggerOperationMethod.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>Enumeration of the available HTTP methods. </summary>
    public enum SwaggerOperationMethod
    {
        /// <summary>The HTTP GET method. </summary>
        [JsonProperty("get")]
        Get,

        /// <summary>The HTTP POST method. </summary>
        [JsonProperty("post")]
        Post,

        /// <summary>The HTTP PUT method. </summary>
        [JsonProperty("put")]
        Put,

        /// <summary>The HTTP DELETE method. </summary>
        [JsonProperty("delete")]
        Delete,

        /// <summary>The HTTP OPTIONS method. </summary>
        [JsonProperty("options")]
        Options,

        /// <summary>The HTTP HEAD method. </summary>
        [JsonProperty("head")]
        Head,

        /// <summary>The HTTP PATCH method. </summary>
        [JsonProperty("patch")]
        Patch
    }
}