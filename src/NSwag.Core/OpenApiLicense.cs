//-----------------------------------------------------------------------
// <copyright file="SwaggerLicense.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The license information.</summary>
    public class OpenApiLicense : JsonExtensionObject
    {
        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Name { get; set; }

        /// <summary>Gets or sets the license URL.</summary>
        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Url { get; set; }
    }
}