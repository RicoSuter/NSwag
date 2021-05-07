//-----------------------------------------------------------------------
// <copyright file="OpenApiComponents.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public OpenApiComponents(OpenApiDocument document)
        {
            var schemas = new ObservableDictionary<string, JsonSchema>();
            schemas.CollectionChanged += (sender, args) =>
            {
                foreach (var pair in schemas.ToArray())
                {
                    if (pair.Value == null)
                    {
                        schemas.Remove(pair.Key);
                    }
                    else
                    {
                        pair.Value.Parent = this;
                    }
                }
            };
            Schemas = schemas;

            var responses = new ObservableDictionary<string, OpenApiResponse>();
            responses.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Responses.Values)
                {
                    path.Parent = document;
                }
            };
            Responses = responses;

            var parameters = new ObservableDictionary<string, OpenApiParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Parameters.Values)
                {
                    path.Parent = document;
                }
            };
            Parameters = parameters;

            Examples = new Dictionary<string, OpenApiExample>();

            var headers = new ObservableDictionary<string, OpenApiParameter>();
            headers.CollectionChanged += (sender, args) =>
            {
                foreach (var pair in headers.ToArray())
                {
                    if (pair.Value == null)
                    {
                        headers.Remove(pair.Key);
                    }
                    else
                    {
                        pair.Value.Parent = this;
                    }
                }
            };
            Headers = headers;

            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
            Links = new Dictionary<string, OpenApiLink>();
            Callbacks = new Dictionary<string, OpenApiCallback>();
        }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "schemas", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema> Schemas { get; }

        /// <summary>Gets or sets the responses which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiResponse> Responses { get; }

        /// <summary>Gets or sets the parameters which can be used for all operations.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiParameter> Parameters { get; }

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "examples", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiExample> Examples { get; set; }

        /// <summary>Gets or sets the types.</summary>
        [JsonProperty(PropertyName = "headers", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiParameter> Headers { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "securitySchemes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "links", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiLink> Links { get; }

        /// <summary>Gets or sets the security definitions.</summary>
        [JsonProperty(PropertyName = "callbacks", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiCallback> Callbacks { get; }
    }
}