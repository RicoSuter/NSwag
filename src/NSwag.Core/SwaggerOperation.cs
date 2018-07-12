//-----------------------------------------------------------------------
// <copyright file="SwaggerOperation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Describes a JSON web service operation. </summary>
    public class SwaggerOperation : JsonExtensionObject
    {
        private OpenApiRequestBody _requestBody;

        private bool _disableRequestBodyUpdate = false;
        private bool _disableBodyParameterUpdate = false;

        /// <summary>Initializes a new instance of the <see cref="SwaggerPathItem"/> class.</summary>
        public SwaggerOperation()
        {
            Tags = new List<string>();

            var parameters = new ObservableCollection<SwaggerParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                foreach (var parameter in Parameters)
                    parameter.Parent = this;

                UpdateRequestBody(args);
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
        [JsonProperty(PropertyName = "tags", Order = 1, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        /// <summary>Gets or sets the summary of the operation.</summary>
        [JsonProperty(PropertyName = "summary", Order = 2, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the long description of the operation.</summary>
        [JsonProperty(PropertyName = "description", Order = 3, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", Order = 4, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Gets or sets the operation ID (unique name).</summary>
        [JsonProperty(PropertyName = "operationId", Order = 5, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string OperationId { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", Order = 6, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Consumes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", Order = 7, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Produces { get; set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonIgnore]
        public IList<SwaggerParameter> Parameters { get; }

        /// <summary>Gets or sets the request body (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "requestBody", Order = 9, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OpenApiRequestBody RequestBody
        {
            get => _requestBody;
            set
            {
                _requestBody = value;
                if (value != null)
                {
                    value.Parent = this;
                    UpdateBodyParameter();
                }
            }
        }

        /// <summary>Gets the actual parameters (a combination of all inherited and local parameters).</summary>
        [JsonIgnore]
        public IReadOnlyList<SwaggerParameter> ActualParameters
        {
            get
            {
                var parameters = Parameters.Select(p => p.ActualParameter);
                var allParameters = Parent?.Parameters == null ? parameters :
                    parameters.Concat(Parent.Parameters)
                    .GroupBy(p => p.Name + "|" + p.Kind)
                    .Select(p => p.First());

                return new ReadOnlyCollection<SwaggerParameter>(allParameters
                    .Select(p => p.ActualSchema is SwaggerParameter ? (SwaggerParameter)p.ActualSchema : p)
                    .ToList());
            }
        }

        /// <summary>Gets or sets the HTTP Status Code/Response pairs.</summary>
        [JsonProperty(PropertyName = "responses", Order = 10, Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses { get; }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<SwaggerSchema> Schemes { get; set; }

        /// <summary>Gets or sets the callbacks (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "callbacks", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>Gets or sets a value indicating whether the operation is deprecated.</summary>
        [JsonProperty(PropertyName = "deprecated", Order = 13, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsDeprecated { get; set; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", Order = 14, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<SwaggerSecurityRequirement> Security { get; set; } = new Collection<SwaggerSecurityRequirement>();

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; set; } = new Collection<OpenApiServer>();

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

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", Order = 8, DefaultValueHandling = DefaultValueHandling.Ignore)]
        internal IList<SwaggerParameter> ParametersRaw
        {
            get
            {
                if (JsonSchemaSerialization.IsWriting)
                    return Parameters;

                if (JsonSchemaSerialization.CurrentSchemaType != SchemaType.Swagger2)
                    return Parameters.Where(p => p.Kind != SwaggerParameterKind.Body).ToList();
                else
                    return Parameters;
            }
        }

        internal void UpdateRequestBody(SwaggerParameter parameter)
        {
            if (!_disableRequestBodyUpdate)
            {
                try
                {
                    _disableBodyParameterUpdate = true;

                    if (parameter.Kind == SwaggerParameterKind.Body)
                    {
                        if (RequestBody == null)
                            RequestBody = new OpenApiRequestBody();

                        RequestBody.Name = parameter.Name;
                        RequestBody.Description = parameter.Description;
                        RequestBody.IsRequired = parameter.IsRequired;

                        RequestBody.Content.Clear();
                        RequestBody.Content.Add(parameter.Schema?.Type == JsonObjectType.File ?
                            "application/octet-stream" : "application/json", new OpenApiMediaType
                            {
                                Schema = parameter.Schema,
                                Example = parameter.Example
                            });
                    }
                }
                finally
                {
                    _disableBodyParameterUpdate = false;
                }
            }
        }

        internal void UpdateBodyParameter()
        {
            if (!_disableBodyParameterUpdate)
            {
                try
                {
                    _disableRequestBodyUpdate = true;

                    var bodyParameter = Parameters.SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);
                    if (bodyParameter != null)
                    {
                        if (RequestBody == null)
                        {
                            var index = Parameters.IndexOf(bodyParameter);
                            Parameters.RemoveAt(index);
                        }
                        else
                            UpdateBodyParameter(bodyParameter);
                    }
                    else
                    {
                        if (RequestBody != null)
                            Parameters.Add(CreateBodyParameter());
                    }
                }
                finally
                {
                    _disableRequestBodyUpdate = false;
                }
            }
        }

        private SwaggerParameter CreateBodyParameter()
        {
            var parameter = new SwaggerParameter();
            UpdateBodyParameter(parameter);
            return parameter;
        }

        private void UpdateBodyParameter(SwaggerParameter parameter)
        {
            parameter.Kind = SwaggerParameterKind.Body;
            parameter.Name = RequestBody.ActualName;
            parameter.Description = RequestBody.Description;
            parameter.IsRequired = RequestBody.IsRequired;
            parameter.Example = RequestBody.Content.FirstOrDefault().Value?.Example;
            parameter.Schema = RequestBody.Content.FirstOrDefault().Value?.Schema;
        }

        private void UpdateRequestBody(NotifyCollectionChangedEventArgs args)
        {
            if (!_disableRequestBodyUpdate)
            {
                var bodyParameter = args
                    .NewItems?
                    .OfType<SwaggerParameter>()
                    .SingleOrDefault(p => p.Kind == SwaggerParameterKind.Body);

                if (bodyParameter != null)
                    UpdateRequestBody(bodyParameter);
                else if (Parameters.All(p => p.Kind != SwaggerParameterKind.Body))
                    RequestBody = null;
            }
        }
    }
}