﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.Collections;

#pragma warning disable 618 // obsolete warning for ToJson

namespace NSwag
{
    /// <summary>Describes a JSON web service.</summary>
    public partial class OpenApiDocument : JsonExtensionObject, IDocumentPathProvider
    {
        internal readonly ObservableDictionary<string, OpenApiPathItem> _paths;

        /// <summary>Initializes a new instance of the <see cref="OpenApiDocument"/> class.</summary>
        public OpenApiDocument()
        {
            Swagger = "2.0";
            OpenApi = "3.0.0";
            Components = new OpenApiComponents(this);

            var paths = new ObservableDictionary<string, OpenApiPathItem>();
            paths.CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Replace)
                {
                    return;
                }

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    var pair = (KeyValuePair<string, OpenApiPathItem>)args.NewItems[i];
                    pair.Value.ActualPathItem.Parent = this;
                }
            };

            _paths = paths;
            Info = new OpenApiInfo();
        }

        private static readonly string _toolChainVersion = typeof(OpenApiDocument).GetTypeInfo().Assembly.GetName().Version.ToString();

        /// <summary>Gets the NSwag toolchain version.</summary>
        public static string ToolchainVersion => _toolChainVersion;

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
        public OpenApiInfo Info { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", Order = 10, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; private set; } = [];

        /// <summary>Gets or sets the operations.</summary>
        [JsonProperty(PropertyName = "paths", Order = 11, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiPathItem> Paths => _paths;

        /// <summary>Gets or sets the components.</summary>
        [JsonProperty(PropertyName = "components", Order = 12, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OpenApiComponents Components { get; }

        /// <summary>Gets or sets a security description.</summary>
        [JsonProperty(PropertyName = "security", Order = 17, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<OpenApiSecurityRequirement> Security { get; set; } = [];

        /// <summary>Gets or sets the description.</summary>
        [JsonProperty(PropertyName = "tags", Order = 18, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IList<OpenApiTag> Tags { get; set; } = [];

        /// <summary>Gets the base URL of the web service.</summary>
        [JsonIgnore]
        public string BaseUrl => Servers?.FirstOrDefault(s => s.IsValid)?.Url ?? "";

        /// <summary>Gets or sets the external documentation.</summary>
        [JsonProperty(PropertyName = "externalDocs", Order = 19, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OpenApiExternalDocumentation ExternalDocumentation { get; set; }

        /// <summary>Converts the Swagger specification to JSON.</summary>
        /// <returns>The JSON string.</returns>
        public string ToJson()
        {
            return ToJson(SchemaType);
        }

        /// <summary>Converts the description object to JSON.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The JSON string.</returns>
        [Obsolete("Do not use this method but only ToJson(). Use the correct generator settings to generate a document in the correct format.")]
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
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromJsonAsync(string data, CancellationToken cancellationToken = default)
        {
            return FromJsonAsync(data, null, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromJsonAsync(string data, string documentPath, CancellationToken cancellationToken = default)
        {
            return FromJsonAsync(data, documentPath, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromJsonAsync(string data, string documentPath,
            SchemaType expectedSchemaType, CancellationToken cancellationToken = default)
        {
            return FromJsonAsync(data, documentPath, expectedSchemaType, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a JSON string.</summary>
        /// <param name="data">The JSON data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="referenceResolverFactory">The JSON reference resolver factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static async Task<OpenApiDocument> FromJsonAsync(string data, string documentPath, SchemaType expectedSchemaType,
            Func<OpenApiDocument, JsonReferenceResolver> referenceResolverFactory, CancellationToken cancellationToken = default)
        {
            // For explanation of the regex use https://regexr.com/ and the below unescaped pattern that is without named groups
            // (?:\"(openapi|swagger)\")(?:\s*:\s*)(?:\"([^"]*)\")
            var pattern = "(?:\\\"(?<schemaType>openapi|swagger)\\\")(?:\\s*:\\s*)(?:\\\"(?<schemaVersion>[^\"]*)\\\")";
            var match = Regex.Match(data, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var schemaType = match.Groups["schemaType"].Value.ToLowerInvariant();
                var schemaVersion = match.Groups["schemaVersion"].Value.ToLowerInvariant();

                if (schemaType == "swagger" && schemaVersion.StartsWith('2'))
                {
                    expectedSchemaType = SchemaType.Swagger2;
                }
                else if (schemaType == "openapi" && schemaVersion.StartsWith('3'))
                {
                    expectedSchemaType = SchemaType.OpenApi3;
                }
            }

            if (expectedSchemaType == SchemaType.JsonSchema)
            {
                throw new NotSupportedException("The schema type JsonSchema is not supported.");
            }

            var contractResolver = GetJsonSerializerContractResolver(expectedSchemaType);
            return await JsonSchemaSerialization.FromJsonAsync<OpenApiDocument>(data, expectedSchemaType, documentPath, document =>
            {
                document.SchemaType = expectedSchemaType;
                if (referenceResolverFactory != null)
                {
                    return referenceResolverFactory(document);
                }
                else
                {
                    var schemaResolver = new OpenApiSchemaResolver(document, new SystemTextJsonSchemaGeneratorSettings());
                    return new JsonReferenceResolver(schemaResolver);
                }
            }, contractResolver, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Creates a Swagger specification from a JSON file.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument" />.</returns>
        public static async Task<OpenApiDocument> FromFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var data = File.ReadAllText(filePath);
            return await FromJsonAsync(data, filePath, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Creates a Swagger specification from an URL.</summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static async Task<OpenApiDocument> FromUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            var data = await DynamicApis.HttpGetAsync(url, cancellationToken).ConfigureAwait(false);
            return await FromJsonAsync(data, url, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Gets the operations.</summary>
        [JsonIgnore]
        public IEnumerable<OpenApiOperationDescription> Operations => GetOperations();

        internal IEnumerable<OpenApiOperationDescription> GetOperations()
        {
            foreach (var p in _paths)
            {
                foreach (var o in p.Value.ActualPathItem)
                {
                    yield return new OpenApiOperationDescription
                    {
                        Path = p.Key,
                        Method = o.Key,
                        Operation = o.Value
                    };
                }
            }
        }

        /// <summary>Generates missing or non-unique operation IDs.</summary>
        public void GenerateOperationIds()
        {
            // start with new work buffers
            GenerateOperationIds([.. GetOperations()], [], []);
        }

        /// <summary>Generates missing or non-unique operation IDs.</summary>
        private static void GenerateOperationIds(
            List<OpenApiOperationDescription> operations,
            HashSet<string> operationIds,
            HashSet<string> duplicatedOperationIds)
        {
            // Generate missing IDs
            operationIds.Clear();
            duplicatedOperationIds.Clear();
            foreach (var operation in operations)
            {
                if (string.IsNullOrEmpty(operation.Operation.OperationId))
                {
                    operation.Operation.OperationId = GetOperationNameFromPath(operation);
                }

                if (!operationIds.Add(operation.Operation.OperationId))
                {
                    duplicatedOperationIds.Add(operation.Operation.OperationId);
                }
            }

            // if we don't have any duplicates, we are done
            if (duplicatedOperationIds.Count == 0)
            {
                return;
            }

            // Find non-unique operation IDs
            operations = [.. operations.Where(x => duplicatedOperationIds.Contains(x.Operation.OperationId))];

            // 1: Append all to methods returning collections
            foreach (var group in operations.GroupBy(o => o.Operation.OperationId))
            {
                if (group.Count() > 1)
                {
                    var collections = group.Where(o => o.Operation.HasActualResponse(static (code, response) =>
                              HttpUtilities.IsSuccessStatusCode(code) &&
                              response.Schema?.ActualSchema.Type == JsonObjectType.Array));
                    // if we have just collections, adding All will not help in discrimination
                    if (collections.Count() == group.Count())
                    {
                        continue;
                    }

                    foreach (var o in group)
                    {
                        var isCollection = o.Operation.HasActualResponse(static (code, response) =>
                            HttpUtilities.IsSuccessStatusCode(code) &&
                            response.Schema?.ActualSchema.Type == JsonObjectType.Array);

                        if (isCollection)
                        {
                            o.Operation.OperationId += "All";
                        }
                    }
                }
            }

            // 2: Append the Method type
            foreach (var group in operations.GroupBy(o => o.Operation.OperationId))
            {
                if (group.Count() > 1)
                {
                    var methods = group.Select(o => o.Method.ToUpperInvariant()).Distinct();
                    if (methods.Count() == 1)
                    {
                        continue;
                    }

                    foreach (var o in group)
                    {
                        o.Operation.OperationId += o.Method.ToUpperInvariant();
                    }
                }
            }

            // 3: Append numbers as last resort
            foreach (var group in operations.GroupBy(o => o.Operation.OperationId))
            {
                var groupOperations = group.ToList();
                if (group.Count() > 1)
                {
                    // Add numbers
                    var i = 2;
                    foreach (var operation in groupOperations.Skip(1))
                    {
                        operation.Operation.OperationId += i++;
                    }

                    GenerateOperationIds(operations, operationIds, duplicatedOperationIds);
                    return;
                }
            }
        }

        private static string GetOperationNameFromPath(OpenApiOperationDescription operation)
        {
            var pathSegments = operation.Path.Trim('/').Split('/');
            var lastPathSegment = pathSegments.LastOrDefault(s => !s.Contains('{'));
            return string.IsNullOrEmpty(lastPathSegment) ? "Anonymous" : lastPathSegment;
        }
    }
}
