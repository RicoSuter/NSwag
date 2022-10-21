//-----------------------------------------------------------------------
// <copyright file="SwaggerResponse.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.References;

namespace NSwag
{
    /// <summary>The Swagger response.</summary>
    public class OpenApiResponse : JsonReferenceBase<OpenApiResponse>, IJsonReference
    {
        // (RegexOptions) 0x0008 = Compiled
        private static readonly Regex AppJsonRegex = new Regex(@"application\/(\S+?)?\+?json;?(\S+)?", (RegexOptions) 0x0008);

        /// <summary>Gets or sets the extension data (i.e. additional properties which are not directly defined by the JSON object).</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        /// <summary>Gets the parent <see cref="OpenApiOperation"/>.</summary>
        [JsonIgnore]
        public object Parent { get; internal set; }

        /// <summary>Gets the actual response, either this or the referenced response.</summary>
        [JsonIgnore]
        public OpenApiResponse ActualResponse => Reference ?? this;

        /// <summary>Gets or sets the response's description.</summary>
        [JsonProperty(PropertyName = "description", Order = 1)]
        public string Description { get; set; } = "";

        /// <summary>Gets or sets the headers.</summary>
        [JsonProperty(PropertyName = "headers", Order = 3, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public OpenApiHeaders Headers { get; } = new OpenApiHeaders();

        /// <summary>Sets a value indicating whether the response can be null (use IsNullable() to get a parameter's nullability).</summary>
        /// <remarks>The Swagger spec does not support null in schemas, see https://github.com/OAI/OpenAPI-Specification/issues/229 </remarks>
        [JsonProperty(PropertyName = "x-nullable", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool? IsNullableRaw { internal get; set; }

        /// <summary>Gets or sets the expected child schemas of the base schema (can be used for generating enhanced typings/documentation).</summary>
        [JsonProperty(PropertyName = "x-expectedSchemas", Order = 7, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<JsonExpectedSchema> ExpectedSchemas { get; set; }

        /// <summary>Gets or sets the descriptions of potential response payloads (OpenApi only).</summary>
        [JsonProperty(PropertyName = "content", Order = 4, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiMediaType> Content { get; } = new Dictionary<string, OpenApiMediaType>();

        /// <summary>Gets or sets the links that can be followed from the response (OpenApi only).</summary>
        [JsonProperty(PropertyName = "links", Order = 5, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public IDictionary<string, OpenApiLink> Links { get; } = new Dictionary<string, OpenApiLink>();

        /// <summary>Gets or sets the response schema (Swagger only).</summary>
        [JsonProperty(PropertyName = "schema", Order = 2, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public JsonSchema Schema
        {
            get => Content.FirstOrDefault(c => c.Value.Schema != null).Value?.Schema;
            set => UpdateContent(value, Examples);
        }

        /// <summary>Gets or sets the headers (Swagger only).</summary>
        [JsonProperty(PropertyName = "examples", Order = 6, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public object Examples
        {
            get => Content.FirstOrDefault(c => c.Value.Example != null).Value?.Example;
            set => UpdateContent(Schema, value);
        }

        private void UpdateContent(JsonSchema schema, object example)
        {
            Content.Clear();

            if (schema != null || example != null)
            {
                var mimeType = schema?.IsBinary == true ? "application/octet-stream" : "application/json";
                Content[mimeType] = new OpenApiMediaType
                {
                    Schema = schema,
                    Example = example
                };
            }
        }

        /// <summary>Determines whether the specified null handling is nullable (fallback value: false).</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <returns>The result.</returns>
        public bool IsNullable(SchemaType schemaType)
        {
            return IsNullable(schemaType, false);
        }

        /// <summary>Determines whether the specified null handling is nullable.</summary>
        /// <param name="schemaType">The schema type.</param>
        /// <param name="fallbackValue">The fallback value when 'x-nullable' is not defined.</param>
        /// <returns>The result.</returns>
        public bool IsNullable(SchemaType schemaType, bool fallbackValue)
        {
            if (schemaType == SchemaType.Swagger2)
            {
                if (IsNullableRaw == null)
                {
                    return fallbackValue;
                }

                return IsNullableRaw.Value;
            }

            return ActualResponse.Schema?.IsNullable(schemaType) ?? false;
        }

        /// <summary>Checks whether this is a binary/file response.</summary>
        /// <param name="operation">The operation the response belongs to.</param>
        /// <returns>The result.</returns>
        public bool IsBinary(OpenApiOperation operation)
        {
            static bool ProducesBinary(ICollection<string> contentTypeKeys)
            {
                foreach (var p in contentTypeKeys)
                {
                    if (p.Contains("application/json") || p.Contains("text/plain") || AppJsonRegex.IsMatch(p))
                    {
                        return false;
                    }
                }

                return true;
            }

            foreach (var r in operation.Responses)
            {
                var key = r.Key;
                var actualResponse = r.Value.ActualResponse;

                if (actualResponse != this || key == "204")
                {
                    continue;
                }

                if (ActualResponse.Content.Count > 0)
                {
                    if (ActualResponse.Content.All(static c => c.Value.Schema?.ActualSchema.IsBinary == true))
                    {
                        return true;
                    }

                    var contentIsBinary =
                        ActualResponse.Content.All(static c =>
                        {
                            var actualSchema = c.Value.Schema?.ActualSchema;
                            return actualSchema?.IsAnyType != false || actualSchema?.IsBinary != false;
                        })
                        && ProducesBinary(ActualResponse.Content.Keys);

                    if (contentIsBinary)
                    {
                        return true;
                    }
                }

                var actualProduces = (ActualResponse.Parent as OpenApiOperation)?.ActualProduces;
                if (actualProduces?.Count > 0)
                {
                    if (Schema?.ActualSchema.IsBinary == true)
                    {
                        return true;
                    }

                    // is binary only if there is no JSON schema defined
                    var producesIsBinary =
                        (Schema?.ActualSchema.IsAnyType != false || Schema?.ActualSchema.IsBinary != false)
                        && ProducesBinary(actualProduces);

                    if (producesIsBinary)
                    {
                        return true;
                    }
                }

                break;
            }

            return false;
        }

        /// <summary>Checks whether this is an empty response.</summary>
        /// <param name="operation">The operation the response belongs to.</param>
        /// <returns>The result.</returns>
        public bool IsEmpty(OpenApiOperation operation)
        {
            return ActualResponse.Content.Count == 0 &&
                   ActualResponse.Schema?.ActualSchema == null &&
                   !IsBinary(operation);
        }

        #region Implementation of IJsonReference

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualResponse;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => (Parent as OpenApiOperation)?.Parent?.Parent;

        #endregion
    }
}
