﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerOperation.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Describes a JSON web service operation. </summary>
    public class OpenApiOperation : JsonExtensionObject
    {
        private OpenApiRequestBody _requestBody;

        private bool _disableRequestBodyUpdate;
        private bool _disableBodyParameterUpdate;

        internal readonly ObservableDictionary<string, OpenApiResponse> _responses;

        /// <summary>Initializes a new instance of the <see cref="OpenApiPathItem"/> class.</summary>
        public OpenApiOperation()
        {
            Tags = [];

            var parameters = new ObservableCollection<OpenApiParameter>();
            parameters.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Replace)
                {
                    return;
                }

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    var parameter = (OpenApiParameter)args.NewItems[i];
                    parameter.Parent = this;
                }

                UpdateRequestBody(args);
            };
            Parameters = parameters;

            var responses = new ObservableDictionary<string, OpenApiResponse>();
            responses.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Replace)
                {
                    return;
                }

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    var pair = (KeyValuePair<string, OpenApiResponse>)args.NewItems[i];
                    pair.Value.Parent = this;
                }
            };
            _responses = responses;
        }

        /// <summary>Gets the parent operations list.</summary>
        [JsonIgnore]
        public OpenApiPathItem Parent { get; internal set; }

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
        public OpenApiExternalDocumentation ExternalDocumentation { get; set; }

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
        public IList<OpenApiParameter> Parameters { get; }

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
        public IReadOnlyList<OpenApiParameter> ActualParameters => [.. GetActualParameters()];

        internal IEnumerable<OpenApiParameter> GetActualParameters()
        {
            var parameters = Parameters.Select(p => p.ActualParameter);
            IEnumerable<OpenApiParameter> allParameters;
            if (Parent?.Parameters == null)
            {
                allParameters = parameters;
            }
            else
            {
                allParameters = parameters
                    .Concat(Parent.Parameters.Select(p => p.ActualParameter))
                    .GroupBy(p => new NameKindPair(p.Name, p.Kind))
                    .Select(p => p.First());
            }

            return allParameters;
        }

        private readonly record struct NameKindPair(string Name, OpenApiParameterKind ParameterKind);

        /// <summary>Gets or sets the HTTP Status Code/Response pairs.</summary>
        [JsonProperty(PropertyName = "responses", Order = 10, Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiResponse> Responses => _responses;

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<OpenApiSchema> Schemes { get; set; }

        /// <summary>Gets or sets the callbacks (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "callbacks", Order = 12, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public IDictionary<string, OpenApiCallback> Callbacks { get; set; } = new Dictionary<string, OpenApiCallback>();

        /// <summary>Gets or sets a value indicating whether the operation is deprecated.</summary>
        [JsonProperty(PropertyName = "deprecated", Order = 13, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsDeprecated { get; set; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", Order = 14, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<OpenApiSecurityRequirement> Security { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", Order = 15, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; set; } = [];

        /// <summary>Gets the list of MIME types the operation can consume, either from the operation or from the <see cref="OpenApiDocument"/>.</summary>
        [JsonIgnore]
        public ICollection<string> ActualConsumes => ActualConsumesCollection;

        [JsonIgnore]
        internal List<string> ActualConsumesCollection => Consumes ?? Parent.Parent._consumes;

        /// <summary>Gets the list of MIME types the operation can produce, either from the operation or from the <see cref="OpenApiDocument"/>.</summary>
        [JsonIgnore]
        public ICollection<string> ActualProduces => ActualProducesCollection;

        [JsonIgnore]
        internal List<string> ActualProducesCollection => Produces ?? Parent.Parent._produces;

        /// <summary>Gets the actual schemes, either from the operation or from the <see cref="OpenApiDocument"/>.</summary>
        [JsonIgnore]
        public ICollection<OpenApiSchema> ActualSchemes => Schemes ?? Parent.Parent.Schemes;

        /// <summary>Gets the response body and dereferences it if necessary.</summary>
        [JsonIgnore]
        public OpenApiRequestBody ActualRequestBody => RequestBody?.ActualRequestBody;

        /// <summary>Gets the responses from the operation and from the <see cref="OpenApiDocument"/> and dereferences them if necessary.</summary>
        [JsonIgnore]
        public IReadOnlyDictionary<string, OpenApiResponse> ActualResponses
        {
            get
            {
                var dictionary = new Dictionary<string, OpenApiResponse>(_responses.Count);
                foreach (var response in _responses)
                {
                    dictionary.Add(response.Key, response.Value.ActualResponse);
                }
                return dictionary;
            }
        }

        // helpers to avoid extra allocations
        internal IEnumerable<KeyValuePair<string, OpenApiResponse>> GetActualResponses(Func<string, OpenApiResponse, bool> predicate)
        {
            foreach (var pair in _responses)
            {
                if (predicate(pair.Key, pair.Value.ActualResponse))
                {
                    yield return new KeyValuePair<string, OpenApiResponse>(pair.Key, pair.Value.ActualResponse);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool HasActualResponse(Func<string, OpenApiResponse, bool> predicate) => GetActualResponse(predicate) != null;

        internal OpenApiResponse GetActualResponse(Func<string, OpenApiResponse, bool> predicate)
        {
            foreach (var pair in _responses)
            {
                if (predicate(pair.Key, pair.Value.ActualResponse))
                {
                    return pair.Value.ActualResponse;
                }
            }

            return null;
        }

        internal KeyValuePair<string,OpenApiResponse> GetSuccessResponse()
        {
            KeyValuePair<string, OpenApiResponse> firstOtherSuccessResponse = new(null, null);
            OpenApiResponse defaultResponse = null;
            foreach (var pair in _responses)
            {
                var code = pair.Key;
                var actualResponse = pair.Value.ActualResponse;

                if (code == "200")
                {
                    // 200 is the default response
                    return new KeyValuePair<string, OpenApiResponse>(code, actualResponse);
                }

                if (firstOtherSuccessResponse.Key == null && HttpUtilities.IsSuccessStatusCode(code))
                {
                    firstOtherSuccessResponse = new KeyValuePair<string, OpenApiResponse>(code, actualResponse);
                }
                else if (code == "default")
                {
                    defaultResponse = actualResponse;
                }
            }

            if (firstOtherSuccessResponse.Key != null)
            {
                return firstOtherSuccessResponse;
            }

            return new KeyValuePair<string, OpenApiResponse>("default", defaultResponse);
        }

        /// <summary>Gets the actual security description, either from the operation or from the <see cref="OpenApiDocument"/>.</summary>
        [JsonIgnore]
        public ICollection<OpenApiSecurityRequirement> ActualSecurity => Security ?? Parent.Parent.Security;

        /// <summary>Adds a consumes MIME type if it does not yet exists.</summary>
        /// <param name="mimeType">The MIME type.</param>
        public void TryAddConsumes(string mimeType)
        {
            if (Consumes == null)
            {
                Consumes = [mimeType];
            }
            else if (!Consumes.Contains(mimeType))
            {
                Consumes.Add(mimeType);
            }
        }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", Order = 8, DefaultValueHandling = DefaultValueHandling.Ignore)]
        internal IList<OpenApiParameter> ParametersRaw
        {
            get
            {
                if (JsonSchemaSerialization.IsWriting)
                {
                    return Parameters;
                }

                if (JsonSchemaSerialization.CurrentSchemaType != SchemaType.Swagger2)
                {
                    return Parameters.Where(p => p.Kind != OpenApiParameterKind.Body).ToList();
                }
                else
                {
                    return Parameters;
                }
            }
        }

        internal void UpdateRequestBody(OpenApiParameter parameter)
        {
            if (!_disableRequestBodyUpdate)
            {
                try
                {
                    _disableBodyParameterUpdate = true;

                    if (parameter.Kind == OpenApiParameterKind.Body)
                    {
                        RequestBody ??= new OpenApiRequestBody();
                        RequestBody.Name = parameter.Name;
                        RequestBody.Position = parameter.Position;
                        RequestBody.Description = parameter.Description;
                        RequestBody.IsRequired = parameter.IsRequired;

                        RequestBody._content.Clear();
                        RequestBody._content.Add(parameter.Schema?.IsBinary == true ?
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

                    var bodyParameter = Parameters.SingleOrDefault(p => p.Kind == OpenApiParameterKind.Body);
                    if (bodyParameter != null)
                    {
                        if (RequestBody == null)
                        {
                            var index = Parameters.IndexOf(bodyParameter);
                            Parameters.RemoveAt(index);
                        }
                        else
                        {
                            UpdateBodyParameter(bodyParameter);
                        }
                    }
                    else
                    {
                        if (RequestBody != null)
                        {
                            Parameters.Add(CreateBodyParameter());
                        }
                    }
                }
                finally
                {
                    _disableRequestBodyUpdate = false;
                }
            }
        }

        private OpenApiParameter CreateBodyParameter()
        {
            var parameter = new OpenApiParameter();
            UpdateBodyParameter(parameter);
            return parameter;
        }

        private void UpdateBodyParameter(OpenApiParameter parameter)
        {
            parameter.Kind = OpenApiParameterKind.Body;
            parameter.Name = RequestBody.ActualName;
            parameter.Position = RequestBody.Position;
            parameter.Description = RequestBody.Description;
            parameter.IsRequired = RequestBody.IsRequired;
            parameter.Example = RequestBody._content.FirstOrDefault().Value?.Example;
            parameter.Schema = RequestBody._content.FirstOrDefault().Value?.Schema;
        }

        private void UpdateRequestBody(NotifyCollectionChangedEventArgs args)
        {
            if (!_disableRequestBodyUpdate)
            {
                var bodyParameter = args
                    .NewItems?
                    .OfType<OpenApiParameter>()
                    .SingleOrDefault(p => p.Kind == OpenApiParameterKind.Body);

                if (bodyParameter != null)
                {
                    UpdateRequestBody(bodyParameter);
                }
                else if (Parameters.All(p => p.Kind != OpenApiParameterKind.Body))
                {
                    RequestBody = null;
                }
            }
        }
    }
}