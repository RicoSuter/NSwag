//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.Serialization.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NSwag
{
    public partial class SwaggerDocument
    {
        /// <summary>Creates the serializer contract resolver based on the <see cref="SchemaType"/>.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The settings.</returns>
        public static PropertyRenameAndIgnoreSerializerContractResolver CreateJsonSerializerContractResolver(SchemaType schemaType)
        {
            var resolver = JsonSchema4.CreateJsonSerializerContractResolver(schemaType);

            if (schemaType == SchemaType.OpenApi3)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "swagger");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "host");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "definitions");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "parameters");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "responses");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "securityDefinitions");

                resolver.IgnoreProperty(typeof(SwaggerResponse), "examples");
            }
            else if (schemaType == SchemaType.Swagger2)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "openapi");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "servers");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "title");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "components");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "examples");
            }

            return resolver;
        }

        private ObservableCollection<SwaggerSchema> _schemes = new ObservableCollection<SwaggerSchema>();

        /// <summary>Gets or sets the base path on which the API is served, which is relative to the <see cref="Host"/>.</summary>
        [JsonProperty(PropertyName = "basePath", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string BasePath
        {
            get
            {
                var segments = Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/').Skip(1);
                return segments != null ? string.Join("/", segments) : null;
            }
            set { UpdateServers(Schemes, Host, value); }
        }

        /// <summary>Gets or sets the host (name or ip) serving the API (Swagger only).</summary>
        [JsonProperty(PropertyName = "host", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Host
        {
            get { return Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/')[0]; }
            set { UpdateServers(Schemes, value, BasePath); }
        }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public ICollection<SwaggerSchema> Schemes
        {
            get
            {
                if (_schemes != null) _schemes.CollectionChanged -= OnSchemesChanged;
                _schemes = new ObservableCollection<SwaggerSchema>(Servers?
                    .Select(s => s.Url.StartsWith("http://") ? SwaggerSchema.Http : SwaggerSchema.Https)
                    .Distinct() ?? new List<SwaggerSchema>());
                _schemes.CollectionChanged += OnSchemesChanged;
                return _schemes;
            }
            set { UpdateServers(value, Host, BaseUrl); }
        }

        private void OnSchemesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateServers(Schemes, Host, BaseUrl);
        }

        private void UpdateServers(ICollection<SwaggerSchema> schemes, string host, string basePath)
        {
            Servers = schemes?.Select(s => new OpenApiServer
            {
                Url = s + host + basePath
            }).ToList() ?? new List<OpenApiServer>();
        }

        /// <summary>Gets or sets the types (Swagger only).</summary>
        [JsonProperty(PropertyName = "definitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Definitions => Components.Schemas;

        /// <summary>Gets or sets the parameters which can be used for all operations (Swagger only).</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerParameter> Parameters => Components.Parameters;

        /// <summary>Gets or sets the responses which can be used for all operations (Swagger only).</summary>
        [JsonProperty(PropertyName = "responses", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses => Components.Responses;

        /// <summary>Gets or sets the security definitions (Swagger only).</summary>
        [JsonProperty(PropertyName = "securityDefinitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, SwaggerSecurityScheme> SecurityDefinitions => Components.SecuritySchemes;

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<string> Consumes { get; set; } = new List<string>();

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<string> Produces { get; set; } = new List<string>();
    }
}
