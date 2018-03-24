//-----------------------------------------------------------------------
// <copyright file="OpenApiComponents.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Container for reusable components (OpenAPI only).</summary>
    public class OpenApiComponents
    {
        /// <summary></summary>
        /// <param name="document"></param>
        public OpenApiComponents(SwaggerDocument document)
        {
            var schemas = new ObservableDictionary<string, JsonSchema4>();
            schemas.CollectionChanged += (sender, args) =>
            {
                foreach (var path in schemas.Values)
                    path.Parent = document;
            };
            Schemas = schemas;

            var responses = new ObservableDictionary<string, SwaggerResponse>();
            responses.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Responses.Values)
                    path.Parent = document;
            };
            Responses = responses;

            var parameters = new ObservableDictionary<string, SwaggerParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Parameters.Values)
                    path.Parent = document;
            };
            Parameters = parameters;

            Examples = new Dictionary<string, OpenApiExample>();

            var headers = new ObservableDictionary<string, JsonSchema4>();
            headers.CollectionChanged += (sender, args) =>
            {
                foreach (var path in headers.Values)
                    path.Parent = document;
            };
            Headers = headers;

            SecuritySchemes = new Dictionary<string, SwaggerSecurityScheme>();
            Links = new Dictionary<string, OpenApiLink>();
            Callbacks = new Dictionary<string, OpenApiCallback>();
        }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "schemas", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Schemas { get; }

        /// <summary>Gets or sets the responses which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses { get; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerParameter> Parameters { get; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiExample> Examples { get; set; }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Headers { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securitySchemes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerSecurityScheme> SecuritySchemes { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiLink> Links { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "callbacks", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiCallback> Callbacks { get; }
    }
}