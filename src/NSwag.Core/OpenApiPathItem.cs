//-----------------------------------------------------------------------
// <copyright file="OpenApiPathItem.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using NJsonSchema.References;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>An OpenApi path, the key is usually a value of <see cref="OpenApiOperationMethod"/>.</summary>
    [JsonConverter(typeof(OpenApiPathItemConverter))]
    public class OpenApiPathItem : ObservableDictionary<string, OpenApiOperation>, IJsonReferenceBase, IJsonReference
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiPathItem"/> class.</summary>
        public OpenApiPathItem()
        {
            CollectionChanged += (sender, args) =>
            {
                if (args.Action != NotifyCollectionChangedAction.Add && args.Action != NotifyCollectionChangedAction.Replace)
                {
                    return;
                }

                for (var i = 0; i < args.NewItems.Count; i++)
                {
                    var pair = (KeyValuePair<string, OpenApiOperation>)args.NewItems[i];
                    pair.Value.Parent = this;
                }
            };
        }

        /// <summary>Gets the parent <see cref="OpenApiDocument"/>.</summary>
        [JsonIgnore]
        public OpenApiDocument Parent { get; internal set; }

        /// <summary>Gets the actual response, either this or the referenced response.</summary>
        [JsonIgnore]
        public OpenApiPathItem ActualPathItem => Reference ?? this;

        /// <summary>Gets or sets the summary (OpenApi only).</summary>
        [JsonPropertyName("summary")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the description (OpenApi only).</summary>
        [JsonPropertyName("description")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Description { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonPropertyName("servers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<OpenApiServer> Servers { get; set; } = [];

        /// <summary>Gets or sets the parameters.</summary>
        [JsonPropertyName("parameters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public ICollection<OpenApiParameter> Parameters { get; set; } = [];

        /// <summary>Gets or sets the extension data (i.e. additional properties which are not directly defined by the JSON object).</summary>
        [JsonExtensionData]
        public IDictionary<string, JsonNode> ExtensionData { get; set; }

        #region Implementation of IJsonReferenceBase and IJsonReference

        private OpenApiPathItem _reference;

        /// <summary>Gets the document path (URI or file path) for resolving relative references.</summary>
        [JsonIgnore]
        public string DocumentPath { get; set; }

        /// <summary>Gets or sets the type reference path ($ref). </summary>
        [JsonPropertyName("$ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        string IJsonReferenceBase.ReferencePath { get; set; }

        /// <summary>Gets or sets the referenced object.</summary>
        [JsonIgnore]
        internal virtual OpenApiPathItem Reference
        {
            get => _reference;
            set
            {
                if (_reference != value)
                {
                    _reference = value;
                    ((IJsonReferenceBase)this).ReferencePath = null;
                }
            }
        }

        /// <summary>Gets or sets the referenced object.</summary>
        [JsonIgnore]
        IJsonReference IJsonReferenceBase.Reference
        {
            get => Reference;
            set => Reference = (OpenApiPathItem)value;
        }

        [JsonIgnore]
        IJsonReference IJsonReference.ActualObject => ActualPathItem;

        [JsonIgnore]
        object IJsonReference.PossibleRoot => Parent;

        #endregion

        // Needed to convert dictionary keys to lower case
        internal sealed class OpenApiPathItemConverter : JsonConverter<OpenApiPathItem>
        {
            public override OpenApiPathItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    return null;
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected StartObject token.");
                }

                var pathItem = new OpenApiPathItem();
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    if (propertyName == "summary")
                    {
                        pathItem.Summary = reader.GetString();
                    }
                    else if (propertyName == "description")
                    {
                        pathItem.Description = reader.GetString();
                    }
                    else if (propertyName == "parameters")
                    {
                        pathItem.Parameters = JsonSerializer.Deserialize<Collection<OpenApiParameter>>(ref reader, options);
                    }
                    else if (propertyName == "servers")
                    {
                        pathItem.Servers = JsonSerializer.Deserialize<Collection<OpenApiServer>>(ref reader, options);
                    }
                    else if (propertyName.StartsWith("x-", StringComparison.OrdinalIgnoreCase))
                    {
                        pathItem.ExtensionData ??= new Dictionary<string, JsonNode>();
                        pathItem.ExtensionData[propertyName] = JsonNode.Parse(ref reader);
                    }
                    else if (propertyName.Contains("$ref"))
                    {
                        var referencePath = reader.GetString();
                        ((IJsonReferenceBase)pathItem).ReferencePath = referencePath;
                    }
                    else
                    {
                        var operation = JsonSerializer.Deserialize<OpenApiOperation>(ref reader, options);
                        pathItem.Add(propertyName, operation);
                    }
                }

                return pathItem;
            }

            public override void Write(Utf8JsonWriter writer, OpenApiPathItem value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                if (value.Summary != null)
                {
                    writer.WriteString("summary", value.Summary);
                }

                if (value.Description != null)
                {
                    writer.WriteString("description", value.Description);
                }

                if (value.ExtensionData != null)
                {
                    foreach (var entry in value.ExtensionData)
                    {
                        writer.WritePropertyName(entry.Key);
                        JsonSerializer.Serialize(writer, entry.Value, options);
                    }
                }

                if (value.Parameters != null && value.Parameters.Count > 0)
                {
                    writer.WritePropertyName("parameters");
                    JsonSerializer.Serialize(writer, value.Parameters, options);
                }

                if (value.Servers != null && value.Servers.Count > 0)
                {
                    writer.WritePropertyName("servers");
                    JsonSerializer.Serialize(writer, value.Servers, options);
                }

                foreach (var pair in value)
                {
                    writer.WritePropertyName(pair.Key.ToLowerInvariant());
                    JsonSerializer.Serialize(writer, pair.Value, options);
                }

                writer.WriteEndObject();
            }
        }
    }
}