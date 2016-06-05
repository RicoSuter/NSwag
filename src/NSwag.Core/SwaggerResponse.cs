//-----------------------------------------------------------------------
// <copyright file="SwaggerResponse.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The Swagger response.</summary>
    public class SwaggerResponse
    {
        /// <summary>Gets or sets the response's description.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the response schema.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "header", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerHeaders Headers { get; set; }

        /// <summary>Gets a value indicating whether the response is nullable.</summary>
        [JsonIgnore]
        public bool IsNullable => false; // TODO: (swagger-problem) Check how to express a nullable response

        /// <summary>Gets the actual non-nullable response schema (either oneOf schema or the actual schema).</summary>
        [JsonIgnore]
        public JsonSchema4 ActualResponseSchema => Schema?.ActualSchema;
    }
}