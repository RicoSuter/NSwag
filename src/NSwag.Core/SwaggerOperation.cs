//-----------------------------------------------------------------------
// <copyright file="SwaggerOperation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Describes a JSON web service operation. </summary>
    public class SwaggerOperation : JsonExtensionObject
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerPathItem"/> class.</summary>
        public SwaggerOperation()
        {
            Tags = new List<string>();

            var parameters = new ObservableCollection<SwaggerParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                foreach (var response in Parameters)
                    response.Parent = this;
            };
            Parameters = parameters;

            var responses = new ObservableDictionary<string, SwaggerResponse>();
            responses.CollectionChanged += (sender, args) =>
            {
                foreach (var response in Responses.Values)
                    response.Parent = this;
            };
            Responses = responses; 
        }

        /// <summary>Gets the parent operations list.</summary>
        [JsonIgnore]
        public SwaggerPathItem Parent { get; internal set; }

        /// <summary>Gets or sets the tags.</summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        /// <summary>Gets or sets the summary of the operation.</summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the long description of the operation.</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Gets or sets the operation ID (unique name).</summary>
        [JsonProperty(PropertyName = "operationId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OperationId { get; set; }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<SwaggerSchema> Schemes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Consumes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Produces { get; set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<SwaggerParameter> Parameters { get; }

        /// <summary>Gets the actual parameters (a combination of all inherited and local parameters).</summary>
        [JsonIgnore]
        public IReadOnlyList<SwaggerParameter> ActualParameters
        {
            get
            {
                var allParameters = Parent?.Parameters == null ? Parameters :
                    Parameters.Concat(Parent.Parameters)
                    .GroupBy(p => p.Name + "|" + p.Kind)
                    .Select(p => p.First());

                return new ReadOnlyCollection<SwaggerParameter>(allParameters
                    .Select(p => p.ActualSchema is SwaggerParameter ? (SwaggerParameter)p.ActualSchema : p)
                    .ToList());
            }
        }

        /// <summary>Gets or sets the HTTP Status Code/Response pairs.</summary>
        [JsonProperty(PropertyName = "responses", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses { get; }

        /// <summary>Gets or sets a value indicating whether the operation is deprecated.</summary>
        [JsonProperty(PropertyName = "deprecated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsDeprecated { get; set; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<SwaggerSecurityRequirement> Security { get; set; } = new Collection<SwaggerSecurityRequirement>();

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; set; } = new Collection<OpenApiServer>();

        /// <summary>Gets or sets the callbacks (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "callbacks", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>Gets the list of MIME types the operation can consume, either from the operation or from the <see cref="SwaggerDocument"/>.</summary>
        [JsonIgnore]
        public IEnumerable<string> ActualConsumes => Consumes ?? Parent.Parent.Consumes;

        /// <summary>Gets the list of MIME types the operation can produce, either from the operation or from the <see cref="SwaggerDocument"/>.</summary>
        [JsonIgnore]
        public IEnumerable<string> ActualProduces => Produces ?? Parent.Parent.Produces;

        /// <summary>Gets the actual schemes, either from the operation or from the <see cref="SwaggerDocument"/>.</summary>
        [JsonIgnore]
        public IEnumerable<SwaggerSchema> ActualSchemes => Schemes ?? Parent.Parent.Schemes;

        /// <summary>Gets the responses from the operation and from the <see cref="SwaggerDocument"/> and dereferences them if necessary.</summary>
        [JsonIgnore]
        public IReadOnlyDictionary<string, SwaggerResponse> ActualResponses => Responses.ToDictionary(t => t.Key, t => t.Value.ActualResponse);

        /// <summary>Gets the actual security description, either from the operation or from the <see cref="SwaggerDocument"/>.</summary>
        [JsonIgnore]
        public ICollection<SwaggerSecurityRequirement> ActualSecurity => Security ?? Parent.Parent.Security;
    }
}