//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>Describes a JSON web service.</summary>
    public partial class SwaggerDocument : JsonExtensionObject, IDocumentPathProvider
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerDocument"/> class.</summary>
        public SwaggerDocument()
        {
            Swagger = "2.0";
            OpenApi = "3.0.0";
            Components = new OpenApiComponents(this);

            var paths = new ObservableDictionary<string, SwaggerPathItem>();
            paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };

            Paths = paths;
            Info = new SwaggerInfo();
        }

        /// <summary>Gets the NSwag toolchain version.</summary>
        public static string ToolchainVersion => typeof(SwaggerDocument).GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>Gets or sets the preferred schema type.</summary>
        [JsonIgnore]
        public SchemaType SchemaType { get; set; } = SchemaType.Swagger2;

        /// <summary>Gets or sets the document path (URI or file path).</summary>
        [JsonIgnore]
        public string DocumentPath { get; set; }

        /// <summary>Gets or sets the Swagger generator information.</summary>
        [JsonProperty(PropertyName = "x-generator", Order = 1, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Generator { get; set; }

        /// <summary>Gets or sets the Swagger specification version being used (Swagger only).</summary>
        [JsonProperty(PropertyName = "swagger", Order = 2, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Swagger { get; set; }

        /// <summary>Gets or sets the OpenAPI specification version being used (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "openapi", Order = 3, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OpenApi { get; set; }

        /// <summary>Gets or sets the metadata about the API.</summary>
        [JsonProperty(PropertyName = "info", Order = 4, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerInfo Info { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; private set; } = new Collection<OpenApiServer>();

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerPathItem> Paths { get; }

        /// <summary>Gets or sets the components.</summary>
        [JsonProperty(PropertyName = "components", Order = 12, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OpenApiComponents Components { get; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", Order = 17, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<SwaggerSecurityRequirement> Security { get; set; } = new Collection<SwaggerSecurityRequirement>();

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", Order = 18, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<SwaggerTag> Tags { get; set; } = new Collection<SwaggerTag>();

        /// <summary>Gets the base URL of the web service.</summary>
        [JsonIgnore]
        public string BaseUrl => Servers?.FirstOrDefault(s => s.IsValid)?.Url ?? "";

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", Order = 19, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Converts the Swagger specification to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public string ToJson()
        {
            return ToJson(SchemaType);
        }

        /// <summary>Converts the description object to JSON.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The JSON string.</returns>
        public string ToJson(SchemaType schemaType)
        {
            return ToJson(schemaType, Formatting.Indented);
        }

        /// <summary>Converts the description object to JSON.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="formatting">The formatting.</param>
        /// <returns>The JSON string.</returns>
        public string ToJson(SchemaType schemaType, Formatting formatting)
        {
            GenerateOperationIds();

            var contractResolver = GetJsonSerializerContractResolver(schemaType);
            return JsonSchemaSerialization.ToJson(this, schemaType, contractResolver, formatting);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static Task<SwaggerDocument> FromJsonAsync(string data)
        {
            return FromJsonAsync(data, null, SchemaType.Swagger2, null);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static Task<SwaggerDocument> FromJsonAsync(string data, string documentPath)
        {
            return FromJsonAsync(data, documentPath, SchemaType.Swagger2, null);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static Task<SwaggerDocument> FromJsonAsync(string data, string documentPath, SchemaType expectedSchemaType)
        {
            return FromJsonAsync(data, documentPath, expectedSchemaType, null);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="referenceResolverFactory">The JSON reference resolver factory.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static async Task<SwaggerDocument> FromJsonAsync(string data, string documentPath, SchemaType expectedSchemaType, Func<SwaggerDocument, JsonReferenceResolver> referenceResolverFactory)
        {
            // For explanation of the regex use https://regexr.com/ and the below unescaped pattern that is without named groups
            // (?:\"(openapi|swagger)\")(?:\s*:\s*)(?:\"([^"]*)\")
            var pattern = "(?:\\\"(?<schemaType>openapi|swagger)\\\")(?:\\s*:\\s*)(?:\\\"(?<schemaVersion>[^\"]*)\\\")";
            var match = Regex.Match(data, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var schemaType = match.Groups["schemaType"].Value.ToLower();
                var schemaVersion = match.Groups["schemaVersion"].Value.ToLower();

                if (schemaType == "swagger" && schemaVersion.StartsWith("2"))
                {
                    expectedSchemaType = SchemaType.Swagger2;
                }
                else if (schemaType == "openapi" && schemaVersion.StartsWith("3"))
                {
                    expectedSchemaType = SchemaType.OpenApi3;
                }
            }

            if (expectedSchemaType == SchemaType.JsonSchema)
            {
                throw new NotSupportedException("The schema type JsonSchema is not supported.");
            }

            var contractResolver = GetJsonSerializerContractResolver(expectedSchemaType);
            return await JsonSchemaSerialization.FromJsonAsync<SwaggerDocument>(data, expectedSchemaType, documentPath, document =>
            {
                document.SchemaType = expectedSchemaType;
                if (referenceResolverFactory != null)
                {
                    return referenceResolverFactory(document);
                }
                else
                {
                    var schemaResolver = new SwaggerSchemaResolver(document, new JsonSchemaGeneratorSettings());
                    return new JsonReferenceResolver(schemaResolver);
                }
            }, contractResolver).ConfigureAwait(false);
        }

        /// <summary>Creates a Swagger specification from a JSON file.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The <see cref="SwaggerDocument" />.</returns>
        public static async Task<SwaggerDocument> FromFileAsync(string filePath)
        {
            var data = await DynamicApis.FileReadAllTextAsync(filePath).ConfigureAwait(false);
            return await FromJsonAsync(data, filePath).ConfigureAwait(false);
        }

        /// <summary>Creates a Swagger specification from an URL.</summary>
        /// <param name="url">The URL.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static async Task<SwaggerDocument> FromUrlAsync(string url)
        {
            var data = await DynamicApis.HttpGetAsync(url).ConfigureAwait(false);
            return await FromJsonAsync(data, url).ConfigureAwait(false);
        }

        /// <summary>Gets the operations.</summary>
        [JsonIgnore]
        public IEnumerable<SwaggerOperationDescription> Operations
        {
            get
            {
                return Paths.SelectMany(p => p.Value.Select(o => new SwaggerOperationDescription
                {
                    Path = p.Key,
                    Method = o.Key,
                    Operation = o.Value
                }));
            }
        }

        /// <summary>Generates missing or non-unique operation IDs.</summary>
        public void GenerateOperationIds()
        {
            // TODO: Improve this method

            // Generate missing IDs
            foreach (var operation in Operations.Where(o => string.IsNullOrEmpty(o.Operation.OperationId)))
                operation.Operation.OperationId = GetOperationNameFromPath(operation);

            // Find non-unique operation IDs
            foreach (var group in Operations.GroupBy(o => o.Operation.OperationId))
            {
                var operations = group.ToList();
                if (group.Count() > 1)
                {
                    // Append "All" if possible
                    var arrayResponseOperation = operations.FirstOrDefault(
                        o => o.Operation.ActualResponses.Any(r => 
                            HttpUtilities.IsSuccessStatusCode(r.Key) && 
                            r.Value.Schema?.ActualSchema.Type == JsonObjectType.Array));

                    if (arrayResponseOperation != null)
                    {
                        var name = arrayResponseOperation.Operation.OperationId + "All";
                        if (Operations.All(o => o.Operation.OperationId != name))
                        {
                            arrayResponseOperation.Operation.OperationId = name;
                            operations.Remove(arrayResponseOperation);
                            GenerateOperationIds();
                            return;
                        }
                    }

                    // Add numbers
                    var i = 2;
                    foreach (var operation in operations.Skip(1))
                        operation.Operation.OperationId += i++;

                    GenerateOperationIds();
                    return;
                }
            }
        }

        private string GetOperationNameFromPath(SwaggerOperationDescription operation)
        {
            var pathSegments = operation.Path.Trim('/').Split('/');
            var lastPathSegment = pathSegments.LastOrDefault(s => !s.Contains("{"));
            return string.IsNullOrEmpty(lastPathSegment) ? "Anonymous" : lastPathSegment;
        }
    }
}
