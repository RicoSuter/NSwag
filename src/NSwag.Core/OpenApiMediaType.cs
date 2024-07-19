﻿//-----------------------------------------------------------------------
// <copyright file="OpenApiMediaType.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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
        private JsonSchema _schema;
        private object _example;

        [JsonIgnore]
        internal OpenApiRequestBody Parent { get; set; }

        /// <summary>Gets or sets the schema.</summary>
        [JsonProperty(PropertyName = "schema", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema Schema
        {
            get => _schema;
            set
            {
                _schema = value;
                Parent?.ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the example.</summary>
        [JsonProperty(PropertyName = "example", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public object Example
        {
            get => _example;
            set
            {
                _example = value;
                Parent?.ParentOperation?.UpdateBodyParameter();
            }
        }

        /// <summary>Gets or sets the headers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiExample> Examples { get; internal set; } = new Dictionary<string, OpenApiExample>();

        /// <summary>Gets or sets the example's value.</summary>
        [JsonProperty(PropertyName = "encoding", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, OpenApiEncoding> Encoding { get; } = new Dictionary<string, OpenApiEncoding>();
    }
}