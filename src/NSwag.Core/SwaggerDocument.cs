//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
            OpenApi = "3.0";

            Info = new SwaggerInfo();
            Schemes = new List<SwaggerSchema>();

            Components = new SwaggerComponents(this);

            var paths = new ObservableDictionary<string, SwaggerOperations>();
            paths.CollectionChanged += (sender, args) =>
            {
                foreach (var path in Paths.Values)
                    path.Parent = this;
            };
            Paths = paths;

            Info = new SwaggerInfo
            {
                Version = string.Empty,
                Title = string.Empty
            };
        }

        /// <summary>Gets the NSwag toolchain version.</summary>
        public static string ToolchainVersion => typeof(SwaggerDocument).GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>Gets the document path (URI or file path).</summary>
        [JsonIgnore]
        public string DocumentPath { get; private set; }

        /// <summary>Gets or sets the Swagger generator information.</summary>
        [JsonProperty(PropertyName = "x-generator", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Generator { get; set; }

        /// <summary>Gets or sets the Swagger specification version being used.</summary>
        [JsonProperty(PropertyName = "swagger", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Swagger { get; set; }

        /// <summary>Gets or sets the OpenAPI specification version being used.</summary>
        [JsonProperty(PropertyName = "openapi", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string OpenApi { get; set; }

        /// <summary>Gets or sets the metadata about the API.</summary>
        [JsonProperty(PropertyName = "info", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SwaggerInfo Info { get; set; }

        /// <summary>Gets or sets the host (name or ip) serving the API.</summary>
        [JsonProperty(PropertyName = "host", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Host { get; set; }

        /// <summary>Gets or sets the base path on which the API is served, which is relative to the <see cref="Host"/>.</summary>
        [JsonProperty(PropertyName = "basePath", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string BasePath { get; set; }

        /// <summary>Gets or sets the schemes.</summary>
        [JsonProperty(PropertyName = "schemes", DefaultValueHandling = DefaultValueHandling.Ignore, ItemConverterType = typeof(StringEnumConverter))]
        public List<SwaggerSchema> Schemes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can consume.</summary>
        [JsonProperty(PropertyName = "consumes", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Consumes { get; set; }

        /// <summary>Gets or sets a list of MIME types the operation can produce.</summary>
        [JsonProperty(PropertyName = "produces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Produces { get; set; }

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, SwaggerOperations> Paths { get; }
        

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

        /// <summary>Gets or sets the components.</summary>
        [JsonProperty(PropertyName = "components", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerComponents Components { get; }


        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerSecurityRequirement> Security { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerTag> Tags { get; set; }

        /// <summary>Gets the base URL of the web service.</summary>
        [JsonIgnore]
        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(Host))
                    return "";

                if (Schemes.Any())
                    return (Schemes.First().ToString().ToLowerInvariant() + "://" + Host + (!string.IsNullOrEmpty(BasePath) ? "/" + BasePath.Trim('/') : string.Empty)).Trim('/');

                return ("http://" + Host + (!string.IsNullOrEmpty(BasePath) ? "/" + BasePath.Trim('/') : string.Empty)).Trim('/');
            }
        }

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SwaggerExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Converts the Swagger specification to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public string ToJson()
        {
            return ToJson(SchemaType.Swagger2);
        }

        /// <summary>Converts the description object to JSON.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The JSON string.</returns>
        public string ToJson(SchemaType schemaType)
        {
            GenerateOperationIds();

            var contractResolver = CreateJsonSerializerContractResolver(schemaType);
            JsonSchemaReferenceUtilities.UpdateSchemaReferencePaths(this, false, contractResolver);

            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return JsonSchemaReferenceUtilities.ConvertPropertyReferences(json);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The <see cref="SwaggerDocument"/>.</returns>
        public static async Task<SwaggerDocument> FromJsonAsync(string data, string documentPath = null, SchemaType schemaType = SchemaType.Swagger2)
        {
            data = JsonSchemaReferenceUtilities.ConvertJsonReferences(data);

            var contractResolver = CreateJsonSerializerContractResolver(schemaType);
            var document = JsonConvert.DeserializeObject<SwaggerDocument>(data, new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                ConstructorHandling = ConstructorHandling.Default,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ContractResolver = contractResolver
            });

            document.DocumentPath = documentPath;

            var schemaResolver = new SwaggerSchemaResolver(document, new JsonSchemaGeneratorSettings());
            var referenceResolver = new JsonReferenceResolver(schemaResolver); 
            await JsonSchemaReferenceUtilities.UpdateSchemaReferencesAsync(document, referenceResolver).ConfigureAwait(false);

            return document;
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
                        a => a.Operation.ActualResponses.Any(r => HttpUtilities.IsSuccessStatusCode(r.Key) && r.Value.ActualResponseSchema != null && r.Value.ActualResponseSchema.Type == JsonObjectType.Array));

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
