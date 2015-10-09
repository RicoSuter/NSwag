//-----------------------------------------------------------------------
// <copyright file="SwaggerSchema.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>The enumeration of Swagger protocol schemes.</summary>
    public enum SwaggerSchema
    {
        /// <summary>The HTTP schema.</summary>
        [JsonProperty("http")]
        Http,

        /// <summary>The HTTPS schema.</summary>
        [JsonProperty("https")]
        Https,

        /// <summary>The WS schema.</summary>
        [JsonProperty("ws")]
        Ws,

        /// <summary>The WSS schema.</summary>
        [JsonProperty("wss")]
        Wss
    }
}