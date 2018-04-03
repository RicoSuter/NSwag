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
        /// <summary>Creates the serializer contract resolver based on the <see cref="NJsonSchema.SchemaType"/>.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The settings.</returns>
        public static PropertyRenameAndIgnoreSerializerContractResolver CreateJsonSerializerContractResolver(SchemaType schemaType)
        {
            var resolver = JsonSchema4.CreateJsonSerializerContractResolver(schemaType);

            if (schemaType == SchemaType.OpenApi3)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "swagger");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "host");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "basePath");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "schemes");

                //resolver.IgnoreProperty(typeof(SwaggerDocument), "consumes");
                //resolver.IgnoreProperty(typeof(SwaggerDocument), "produces");

                resolver.IgnoreProperty(typeof(SwaggerOperation), "schemes");
                resolver.IgnoreProperty(typeof(SwaggerOperation), "consumes");
                resolver.IgnoreProperty(typeof(SwaggerOperation), "produces");

                //resolver.IgnoreProperty(typeof(SwaggerResponse), "consumes"); => TODO map to response.content
                //resolver.IgnoreProperty(typeof(SwaggerResponse), "produces");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "definitions");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "parameters");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "responses");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "securityDefinitions");

                resolver.IgnoreProperty(typeof(SwaggerResponse), "schema");
                resolver.IgnoreProperty(typeof(SwaggerResponse), "examples");
            }
            else if (schemaType == SchemaType.Swagger2)
            {
                resolver.IgnoreProperty(typeof(SwaggerDocument), "openapi");
                resolver.IgnoreProperty(typeof(SwaggerDocument), "servers");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "title");

                // TODO: Use rename for not mapped properties!
                resolver.IgnoreProperty(typeof(SwaggerPathItem), "summary");
                resolver.IgnoreProperty(typeof(SwaggerPathItem), "description");
                resolver.IgnoreProperty(typeof(SwaggerPathItem), "servers");

                resolver.IgnoreProperty(typeof(SwaggerOperation), "callbacks");
                resolver.IgnoreProperty(typeof(SwaggerOperation), "servers");
                resolver.IgnoreProperty(typeof(SwaggerOperation), "requestBody");

                resolver.IgnoreProperty(typeof(SwaggerDocument), "components");
                resolver.IgnoreProperty(typeof(SwaggerParameter), "examples");

                resolver.IgnoreProperty(typeof(SwaggerResponse), "content");
                resolver.IgnoreProperty(typeof(SwaggerResponse), "links");
            }

            return resolver;
        }

        private ObservableCollection<SwaggerSchema> _schemes = new ObservableCollection<SwaggerSchema>();

        /// <summary>Gets or sets the host (name or ip) serving the API (Swagger only).</summary>
        [JsonProperty(PropertyName = "host", Order = 5, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Host
        {
            get { return Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/')[0]; }
            set { UpdateServers(Schemes, value, BasePath); }
        }

        /// <summary>Gets or sets the base path on which the API is served, which is relative to the <see cref="Host"/>.</summary>
        [JsonProperty(PropertyName = "basePath", Order = 6, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string BasePath
        {
            get
            {
                var segments = Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/').Skip(1).ToArray();
                return segments != null && segments.Length > 0 ? "/" + string.Join("/", segments) : null;
            }
            set { UpdateServers(Schemes, Host, value); }
        }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", Order = 7, DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public ICollection<SwaggerSchema> Schemes
        {
            get
            {
                if (_schemes != null) _schemes.CollectionChanged -= OnSchemesChanged;
                _schemes = new ObservableCollection<SwaggerSchema>(Servers?
                    .Where(s => s.Url.Contains("://"))
                    .Select(s => s.Url.StartsWith("http://") ? SwaggerSchema.Http : SwaggerSchema.Https)
                    .Distinct() ?? new List<SwaggerSchema>());
                _schemes.CollectionChanged += OnSchemesChanged;
                return _schemes;
            }
            set { UpdateServers(value, Host, BasePath); }
        }

        private void OnSchemesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateServers((ICollection<SwaggerSchema>)sender, Host, BasePath);
        }

        private void UpdateServers(ICollection<SwaggerSchema> schemes, string host, string basePath)
        {
            if ((schemes == null || schemes.Count == 0) && (!string.IsNullOrEmpty(host) || !string.IsNullOrEmpty(basePath)))
            {
                Servers = new List<OpenApiServer>
                {
                    new OpenApiServer
                    {
                        Url = host + basePath
                    }
                };
            }
            else
            {
                Servers = schemes?.Select(s => new OpenApiServer
                {
                    Url = s.ToString().ToLowerInvariant() + "://" + host + basePath
                }).ToList() ?? new List<OpenApiServer>();
            }
        }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", Order = 8, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<string> Consumes { get; set; } = new List<string>();

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", Order = 9, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<string> Produces { get; set; } = new List<string>();

        /// <summary>Gets or sets the types (Swagger only).</summary>
        [JsonProperty(PropertyName = "definitions", Order = 13, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, JsonSchema4> Definitions => Components.Schemas;

        /// <summary>Gets or sets the parameters which can be used for all operations (Swagger only).</summary>
        [JsonProperty(PropertyName = "parameters", Order = 14, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerParameter> Parameters => Components.Parameters;

        /// <summary>Gets or sets the responses which can be used for all operations (Swagger only).</summary>
        [JsonProperty(PropertyName = "responses", Order = 15, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerResponse> Responses => Components.Responses;

        /// <summary>Gets or sets the security definitions (Swagger only).</summary>
        [JsonProperty(PropertyName = "securityDefinitions", Order = 16, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerSecurityScheme> SecurityDefinitions => Components.SecuritySchemes;
    }
}
