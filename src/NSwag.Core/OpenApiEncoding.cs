//-----------------------------------------------------------------------
// <copyright file="OpenApiEncoding.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace NSwag
{
    /// <summary>Describes the OpenApi encoding.</summary>
    public class OpenApiEncoding
    {
        /// <summary>Gets or sets the encoding type.</summary>
        [JsonPropertyName("encodingType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EncodingType { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonPropertyName("headers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OpenApiHeaders Headers { get; } = [];

        /// <summary>Gets or sets the encoding type.</summary>
        [JsonPropertyName("style")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Style { get; set; }

        /// <summary>Gets or sets a value indicating whether values of type array or object generate separate parameters for each value of the array, or key-value-pair of the map.</summary>
        [JsonPropertyName("explode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool Explode { get; set; }

        /// <summary>Gets or sets a value indicating whether the parameter value should allow reserved characters, as defined by RFC3986.</summary>
        [JsonPropertyName("allowReserved")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool AllowReserved { get; set; }
    }
}