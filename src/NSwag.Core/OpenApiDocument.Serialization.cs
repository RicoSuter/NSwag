//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.Serialization.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Infrastructure;

namespace NSwag
{
    public partial class OpenApiDocument
    {
        private static readonly Lazy<SchemaSerializationConverter> Swagger2Converter =
            new Lazy<SchemaSerializationConverter>(() => CreateSchemaSerializationConverter(SchemaType.Swagger2));

        private static readonly Lazy<SchemaSerializationConverter> OpenApi3Converter =
            new Lazy<SchemaSerializationConverter>(() => CreateSchemaSerializationConverter(SchemaType.OpenApi3));

        /// <summary>Creates the schema serialization converter based on the <see cref="NJsonSchema.SchemaType"/>.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The converter.</returns>
        public static SchemaSerializationConverter GetSchemaSerializationConverter(SchemaType schemaType)
        {
            if (schemaType == SchemaType.Swagger2)
            {
                return Swagger2Converter.Value;
            }
            else if (schemaType == SchemaType.OpenApi3)
            {
                return OpenApi3Converter.Value;
            }

            throw new ArgumentException("The schema type '" + schemaType + "' is not supported.");
        }

        private static SchemaSerializationConverter CreateSchemaSerializationConverter(SchemaType schemaType)
        {
            var converter = JsonSchema.CreateSchemaSerializationConverter(schemaType);

            // Add custom converter for OpenApiParameter to handle the "required" property
            // collision between OpenApiParameter.IsRequired (bool) and JsonSchema.RequiredPropertiesRaw (string[]).
            converter.AddConverter(new Converters.OpenApiParameterJsonConverter());

            // Register additional types for empty collection filtering (no specific ignores needed,
            // but registering them ensures the converter handles them for empty array stripping).
            converter.IgnoreProperty(typeof(OpenApiComponents));
            converter.IgnoreProperty(typeof(OpenApiServer));
            converter.IgnoreProperty(typeof(OpenApiOAuthFlows));
            converter.IgnoreProperty(typeof(OpenApiOAuthFlow));
            converter.IgnoreProperty(typeof(OpenApiMediaType));
            converter.IgnoreProperty(typeof(OpenApiLink));
            converter.IgnoreProperty(typeof(OpenApiRequestBody));
            converter.IgnoreProperty(typeof(OpenApiEncoding));
            converter.IgnoreProperty(typeof(OpenApiInfo));
            converter.IgnoreProperty(typeof(OpenApiTag));
            converter.IgnoreProperty(typeof(OpenApiExample));
            converter.IgnoreProperty(typeof(OpenApiExternalDocumentation));
            converter.IgnoreProperty(typeof(OpenApiContact));
            converter.IgnoreProperty(typeof(OpenApiLicense));
            converter.IgnoreProperty(typeof(OpenApiServerVariable));
            converter.IgnoreProperty(typeof(OpenApiHeader));
            // Note: OpenApiCallback and OpenApiPathItem implement IDictionary and have their own
            // serialization logic, so they're not registered with the SchemaSerializationConverter.

            if (schemaType == SchemaType.Swagger2)
            {
                converter.IgnoreProperty(typeof(OpenApiDocument), "openapi");
                converter.IgnoreProperty(typeof(OpenApiDocument), "servers");
                converter.IgnoreProperty(typeof(OpenApiParameter), "title");

                // TODO: Use rename for not mapped properties!
                converter.IgnoreProperty(typeof(OpenApiPathItem), "summary");
                converter.IgnoreProperty(typeof(OpenApiPathItem), "description");
                converter.IgnoreProperty(typeof(OpenApiPathItem), "servers");

                converter.IgnoreProperty(typeof(OpenApiOperation), "callbacks");
                converter.IgnoreProperty(typeof(OpenApiOperation), "servers");
                converter.IgnoreProperty(typeof(OpenApiOperation), "requestBody");

                converter.IgnoreProperty(typeof(OpenApiDocument), "components");
                converter.IgnoreProperty(typeof(OpenApiParameter), "examples");
                converter.IgnoreProperty(typeof(OpenApiParameter), "x-position");

                converter.IgnoreProperty(typeof(OpenApiResponse), "content");
                converter.IgnoreProperty(typeof(OpenApiResponse), "links");

                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "scheme");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "bearerFormat");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "openIdConnectUrl");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "flows");
            }
            else if (schemaType == SchemaType.OpenApi3)
            {
                converter.IgnoreProperty(typeof(OpenApiDocument), "swagger");

                converter.IgnoreProperty(typeof(OpenApiDocument), "host");
                converter.IgnoreProperty(typeof(OpenApiDocument), "basePath");
                converter.IgnoreProperty(typeof(OpenApiDocument), "schemes");

                converter.IgnoreProperty(typeof(OpenApiDocument), "consumes");
                converter.IgnoreProperty(typeof(OpenApiDocument), "produces");

                converter.IgnoreProperty(typeof(OpenApiOperation), "schemes");
                converter.IgnoreProperty(typeof(OpenApiOperation), "consumes");
                converter.IgnoreProperty(typeof(OpenApiOperation), "produces");

                //converter.IgnoreProperty(typeof(SwaggerParameter), "x-nullable");

                //converter.IgnoreProperty(typeof(SwaggerResponse), "consumes"); => TODO map to response.content
                //converter.IgnoreProperty(typeof(SwaggerResponse), "produces");

                converter.IgnoreProperty(typeof(OpenApiDocument), "definitions");
                converter.IgnoreProperty(typeof(OpenApiDocument), "parameters");
                converter.IgnoreProperty(typeof(OpenApiDocument), "responses");
                converter.IgnoreProperty(typeof(OpenApiDocument), "securityDefinitions");

                converter.IgnoreProperty(typeof(OpenApiResponse), "schema");
                converter.IgnoreProperty(typeof(OpenApiResponse), "examples");
                converter.IgnoreProperty(typeof(OpenApiResponse), "x-nullable");

                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "flow");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "authorizationUrl");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "tokenUrl");
                converter.IgnoreProperty(typeof(OpenApiSecurityScheme), "scopes");
            }
            else
            {
                throw new ArgumentException("The given schema type is not supported.");
            }

            return converter;
        }

        private ObservableCollection<OpenApiSchema> _schemes = [];
        internal List<string> _consumes = [];
        internal List<string> _produces = [];

        /// <summary>Gets or sets the host (name or ip) serving the API (Swagger only).</summary>
        [JsonPropertyName("host")]
        [JsonPropertyOrder(5)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Host
        {
            get => Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/')[0];
            set => UpdateServers(Schemes, value, BasePath);
        }

        /// <summary>Gets or sets the base path on which the API is served, which is relative to the <see cref="Host"/>.</summary>
        [JsonPropertyName("basePath")]
        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string BasePath
        {
            get
            {
                var segments = Servers?.FirstOrDefault()?.Url?.Replace("http://", "").Replace("https://", "").Split('/').Skip(1).ToArray();
                return segments != null && segments.Length > 0 ? "/" + string.Join("/", segments) : null;
            }
            set => UpdateServers(Schemes, Host, value);
        }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonPropertyName("schemes")]
        [JsonPropertyOrder(7)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<OpenApiSchema> Schemes
        {
            get
            {
                if (_schemes != null)
                {
                    _schemes.CollectionChanged -= OnSchemesChanged;
                }

                _schemes = new ObservableCollection<OpenApiSchema>(Servers?
                    .Where(s => s.Url.Contains("://"))
                    .Select(s => s.Url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? OpenApiSchema.Http : OpenApiSchema.Https)
                    .Distinct() ?? []);

                _schemes.CollectionChanged += OnSchemesChanged;

                return _schemes;
            }
            set => UpdateServers(value, Host, BasePath);
        }

        private void OnSchemesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateServers((ICollection<OpenApiSchema>)sender, Host, BasePath);
        }

        private void UpdateServers(ICollection<OpenApiSchema> schemes, string host, string basePath)
        {
            if ((schemes == null || schemes.Count == 0) && (!string.IsNullOrEmpty(host) || !string.IsNullOrEmpty(basePath)))
            {
                Servers =
                [
                    new OpenApiServer
                    {
                        Url = host + basePath
                    }
                ];
            }
            else
            {
                Servers = schemes?.Select(s => new OpenApiServer
                {
                    Url = s.ToString().ToLowerInvariant() + "://" + host + basePath
                }).ToList() ?? [];
            }
        }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonPropertyName("consumes")]
        [JsonPropertyOrder(8)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<string> Consumes
        {
            get => _consumes;
            set => _consumes = value as List<string> ?? [..value ?? []];
        }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonPropertyName("produces")]
        [JsonPropertyOrder(9)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<string> Produces
        {
            get => _produces;
            set => _produces = value as List<string> ?? [..value ?? []];
        }

        /// <summary>Gets or sets the types (Swagger only).</summary>
        [JsonPropertyName("definitions")]
        [JsonPropertyOrder(13)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, JsonSchema> Definitions => Components.Schemas;

        /// <summary>Gets or sets the parameters which can be used for all operations (Swagger only).</summary>
        [JsonPropertyName("parameters")]
        [JsonPropertyOrder(14)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, OpenApiParameter> Parameters => Components.Parameters;

        /// <summary>Gets or sets the responses which can be used for all operations (Swagger only).</summary>
        [JsonPropertyName("responses")]
        [JsonPropertyOrder(15)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, OpenApiResponse> Responses => Components.Responses;

        /// <summary>Gets or sets the security definitions (Swagger only).</summary>
        [JsonPropertyName("securityDefinitions")]
        [JsonPropertyOrder(16)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, OpenApiSecurityScheme> SecurityDefinitions => Components.SecuritySchemes;
    }
}
