//-----------------------------------------------------------------------
// <copyright file="SwaggerYamlDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Dynamic;
using System.Text.Json.Nodes;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NJsonSchema.Yaml;
using YamlDotNet.Serialization;

namespace NSwag
{
    /// <summary>Extension methods to load and save <see cref="OpenApiDocument"/> from/to YAML.</summary>
    public static class OpenApiYamlDocument
    {
        /// <summary>Creates a Swagger specification from a YAML string.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(string data, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, null, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML string.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(string data, string documentPath, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, documentPath, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML string.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(string data, string documentPath, SchemaType expectedSchemaType, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, documentPath, expectedSchemaType, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML string.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="referenceResolverFactory">The JSON reference resolver factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static async Task<OpenApiDocument> FromYamlAsync(string data, string documentPath,
            SchemaType expectedSchemaType,
            Func<OpenApiDocument, JsonReferenceResolver> referenceResolverFactory,
            CancellationToken cancellationToken = default)
            => await FromYamlAsync(new StringReader(data), documentPath, expectedSchemaType, referenceResolverFactory,
                cancellationToken);

        /// <summary>Creates a Swagger specification from a YAML text stream.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(TextReader data, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, null, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML text stream.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(TextReader data, string documentPath, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, documentPath, SchemaType.Swagger2, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML text stream.</summary>
        /// <param name="data">The JSON or YAML data.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static Task<OpenApiDocument> FromYamlAsync(TextReader data, string documentPath, SchemaType expectedSchemaType, CancellationToken cancellationToken = default)
        {
            return FromYamlAsync(data, documentPath, expectedSchemaType, null, cancellationToken);
        }

        /// <summary>Creates a Swagger specification from a YAML text stream.</summary>
        /// <param name="data">The JSON or YAML data as a reader.</param>
        /// <param name="documentPath">The document path (URL or file path) for resolving relative document references.</param>
        /// <param name="expectedSchemaType">The expected schema type which is used when the type cannot be determined.</param>
        /// <param name="referenceResolverFactory">The JSON reference resolver factory.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static async Task<OpenApiDocument> FromYamlAsync(TextReader data, string documentPath, SchemaType expectedSchemaType,
            Func<OpenApiDocument, JsonReferenceResolver> referenceResolverFactory, CancellationToken cancellationToken = default)
        {
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(data);
            var serializer = new SerializerBuilder()
                .JsonCompatible()
                .Build();

            var json = serializer.Serialize(yamlObject);

            referenceResolverFactory ??= CreateReferenceResolverFactory();
            return await OpenApiDocument.FromJsonAsync(json, documentPath, expectedSchemaType, referenceResolverFactory, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Converts the Swagger specification to YAML.</summary>
        /// <returns>The YAML string.</returns>
        public static string ToYaml(this OpenApiDocument document)
        {
            var json = document.ToJson();
            var jsonNode = JsonNode.Parse(json);
            var expandoObject = ConvertJsonNodeToExpandoObject(jsonNode);

            var serializer = new Serializer();
            return serializer.Serialize(expandoObject);
        }

        /// <summary>Creates a Swagger specification from a JSON file.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument" />.</returns>
        public static async Task<OpenApiDocument> FromFileAsync(string filePath, CancellationToken cancellationToken = default)
        {
            var data = File.ReadAllText(filePath);
            return await FromYamlAsync(data, filePath, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>Creates a Swagger specification from an URL.</summary>
        /// <param name="url">The URL.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="OpenApiDocument"/>.</returns>
        public static async Task<OpenApiDocument> FromUrlAsync(string url, CancellationToken cancellationToken = default)
        {
            var data = await DynamicApis.HttpGetAsync(url, cancellationToken).ConfigureAwait(false);
            return await FromYamlAsync(data, url, cancellationToken).ConfigureAwait(false);
        }

        private static object ConvertJsonNodeToExpandoObject(JsonNode node)
        {
            if (node is JsonObject jsonObject)
            {
                var expando = new ExpandoObject();
                var dict = (IDictionary<string, object>)expando;
                foreach (var property in jsonObject)
                {
                    dict[property.Key] = property.Value != null ? ConvertJsonNodeToExpandoObject(property.Value) : null;
                }
                return expando;
            }
            else if (node is JsonArray jsonArray)
            {
                return jsonArray.Select(item => item != null ? ConvertJsonNodeToExpandoObject(item) : null).ToList();
            }
            else if (node is JsonValue jsonValue)
            {
                if (jsonValue.TryGetValue<bool>(out var boolValue)) return boolValue;
                if (jsonValue.TryGetValue<long>(out var longValue)) return longValue;
                if (jsonValue.TryGetValue<double>(out var doubleValue)) return doubleValue;
                if (jsonValue.TryGetValue<string>(out var stringValue)) return stringValue;
                return node.ToJsonString();
            }
            return null;
        }

        private static Func<OpenApiDocument, JsonReferenceResolver> CreateReferenceResolverFactory()
        {
            return document =>
            {
                var schemaResolver = new OpenApiSchemaResolver(document, new SystemTextJsonSchemaGeneratorSettings());
                return new JsonAndYamlReferenceResolver(schemaResolver);
            };
        }
    }
}
