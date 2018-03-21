//-----------------------------------------------------------------------
// <copyright file="OpenApiMediaType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema;

namespace NSwag
{
    /// <summary>The Swagger media type (OpenAPI only).</summary>
    public class OpenApiMediaType
    {
        /// <summary>Gets or sets the schema.</summary>
        [JsonProperty(PropertyName = "schema", Order = 2, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema4 Schema { get; set; }

        /// <summary>Gets or sets the example.</summary>
        [JsonProperty(PropertyName = "example", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Example { get; set; }

        /// <summary>Gets or sets the headers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiExample> Examples { get; } = new Dictionary<string, OpenApiExample>();

        /// <summary>Gets or sets the example's value.</summary>
        [JsonProperty(PropertyName = "encoding", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Encoding { get; set; } // TODO: Implement model
    }
}