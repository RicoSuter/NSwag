﻿//-----------------------------------------------------------------------
// <copyright file="OpenApiPathItem.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;
using Newtonsoft.Json;
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
                foreach (var operation in Values)
                {
                    operation.Parent = this;
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
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the description (OpenApi only).</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; set; } = [];

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<OpenApiParameter> Parameters { get; set; } = [];

        /// <summary>Gets or sets the extension data (i.e. additional properties which are not directly defined by the JSON object).</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        #region Implementation of IJsonReferenceBase and IJsonReference

        private OpenApiPathItem _reference;

        /// <summary>Gets the document path (URI or file path) for resolving relative references.</summary>
        [JsonIgnore]
        public string DocumentPath { get; set; }

        /// <summary>Gets or sets the type reference path ($ref). </summary>
        [JsonProperty("$ref", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
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
        internal sealed class OpenApiPathItemConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var operations = (OpenApiPathItem)value;
                writer.WriteStartObject();

                if (operations.Summary != null)
                {
                    writer.WritePropertyName("summary");
                    serializer.Serialize(writer, operations.Summary);
                }

                if (operations.Description != null)
                {
                    writer.WritePropertyName("description");
                    serializer.Serialize(writer, operations.Description);
                }

                if (operations.ExtensionData != null)
                {
                    foreach (var tuple in operations.ExtensionData)
                    {
                        writer.WritePropertyName(tuple.Key);
                        serializer.Serialize(writer, tuple.Value);
                    }
                }

                if (operations.Parameters != null && operations.Parameters.Count > 0)
                {
                    writer.WritePropertyName("parameters");
                    serializer.Serialize(writer, operations.Parameters);
                }

                if (operations.Servers != null && operations.Servers.Count > 0)
                {
                    writer.WritePropertyName("servers");
                    serializer.Serialize(writer, operations.Servers);
                }

                foreach (var pair in operations)
                {
                    writer.WritePropertyName(pair.Key.ToString().ToLowerInvariant());
                    serializer.Serialize(writer, pair.Value);
                }

                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }

                var operations = new OpenApiPathItem();
                while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();
                    reader.Read();

                    if (propertyName == "summary")
                    {
                        operations.Summary = (string)serializer.Deserialize(reader, typeof(string));
                    }
                    else if (propertyName == "description")
                    {
                        operations.Description = (string)serializer.Deserialize(reader, typeof(string));
                    }
                    else if (propertyName == "parameters")
                    {
                        operations.Parameters = (Collection<OpenApiParameter>)serializer.Deserialize(reader, typeof(Collection<OpenApiParameter>));
                    }
                    else if (propertyName == "servers")
                    {
                        operations.Servers = (Collection<OpenApiServer>)serializer.Deserialize(reader, typeof(Collection<OpenApiServer>));
                    }
                    else if (propertyName.StartsWith("x-", StringComparison.OrdinalIgnoreCase))
                    {
                        operations.ExtensionData ??= new Dictionary<string, object>();
                        operations.ExtensionData[propertyName] = serializer.Deserialize(reader);
                    }
                    else if (propertyName.Contains("$ref"))
                    {
                        string refPath = serializer.Deserialize(reader).ToString();
                        ((IJsonReferenceBase)operations).ReferencePath = refPath;
                    }
                    else
                    {
                        var value = (OpenApiOperation)serializer.Deserialize(reader, typeof(OpenApiOperation));
                        operations.Add(propertyName, value);
                    }
                }
                return operations;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(OpenApiPathItem);
            }
        }
    }
}