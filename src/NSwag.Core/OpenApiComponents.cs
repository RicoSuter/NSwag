//-----------------------------------------------------------------------
// <copyright file="SwaggerComponents.cs" company="NSwag">
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
            SecuritySchemes = new Dictionary<string, SwaggerSecurityScheme>();
            Examples = new Dictionary<string, SwaggerExample>();

            var schemas = new ObservableDictionary<string, JsonSchema4>();
            schemas.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Schemas.Values)
                    path.Parent = document;
            };
            Schemas = schemas;

            var headers = new ObservableDictionary<string, JsonSchema4>();
            headers.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Schemas.Values)
                    path.Parent = document;
            };
            Headers = headers;

            var parameters = new ObservableDictionary<string, SwaggerParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Parameters.Values)
                    path.Parent = document;
            };
            Parameters = parameters;

            var responses = new ObservableDictionary<string, SwaggerResponse>();
            responses.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Responses.Values)
                    path.Parent = document;
            };
            Responses = responses;
        }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "schemas", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Schemas { get; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerParameter> Parameters { get; }

        /// <summary>Gets or sets the responses which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securitySchemes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, SwaggerSecurityScheme> SecuritySchemes { get; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerExample> Examples { get; set; }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Headers { get; }
    }
}