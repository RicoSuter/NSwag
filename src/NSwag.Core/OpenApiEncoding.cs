//-----------------------------------------------------------------------
// <copyright file="OpenApiEncoding.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;

namespace NSwag
{
    /// <summary>Describes the OpenApi encoding.</summary>
    public class OpenApiEncoding
    {
        /// <summary>Gets or sets the encoding type.</summary>
        [JsonProperty(PropertyName = "encodingType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string EncodingType { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerHeaders Headers { get; } = new SwaggerHeaders();

        /// <summary>Gets or sets the encoding type.</summary>
        [JsonProperty(PropertyName = "style", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Style { get; set; }

        /// <summary>Gets or sets a value indicating whether values of type array or object generate separate parameters for each value of the array, or key-value-pair of the map.</summary>
        [JsonProperty(PropertyName = "explode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Explode { get; set; }

        /// <summary>Gets or sets a value indicating whether the parameter value should allow reserved characters, as defined by RFC3986.</summary>
        [JsonProperty(PropertyName = "allowReserved", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AllowReserved { get; set; }
    }
}