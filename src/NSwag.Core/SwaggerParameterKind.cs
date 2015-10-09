//-----------------------------------------------------------------------
// <copyright file="SwaggerParameterKind.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>Enumeration of the parameter kinds. </summary>
    public enum SwaggerParameterKind
    {
        /// <summary>An undefined kind.</summary>
        Undefined,

        /// <summary>A JSON object as POST or PUT body (only one parameter of this type is allowed). </summary>
        [JsonProperty("body")]
        Body,

        /// <summary>A query key-value pair. </summary>
        [JsonProperty("query")]
        Query,

        /// <summary>An URL path placeholder. </summary>
        [JsonProperty("path")]
        Path,

        /// <summary>A HTTP header parameter.</summary>
        [JsonProperty("header")]
        Header,

        /// <summary>A form data parameter.</summary>
        [JsonProperty("formData")]
        FormData
    }
}