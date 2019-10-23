//-----------------------------------------------------------------------
// <copyright file="SwaggerPathItem.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>A Swagger path, the key is usually a value of <see cref="OpenApiOperationMethod"/>.</summary>
    [JsonConverter(typeof(OpenApiPathItemConverter))]
    public class OpenApiPathItem : ObservableDictionary<string, OpenApiOperation>
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

        /// <summary>Gets or sets the summary (OpenApi only).</summary>
        [JsonProperty(PropertyName = "summary", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Summary { get; set; }

        /// <summary>Gets or sets the description (OpenApi only).</summary>
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Description { get; set; }

        /// <summary>Gets or sets the servers (OpenAPI only).</summary>
        [JsonProperty(PropertyName = "servers", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public ICollection<OpenApiServer> Servers { get; set; } = new Collection<OpenApiServer>();

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ICollection<OpenApiParameter> Parameters { get; set; } = new Collection<OpenApiParameter>();

        /// <summary>Gets or sets the extension data (i.e. additional properties which are not directly defined by the JSON object).</summary>
        [JsonExtensionData]
        public IDictionary<string, object> ExtensionData { get; set; }

        // Needed to convert dictionary keys to lower case
        internal class OpenApiPathItemConverter : JsonConverter
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

                if (operations.Parameters != null && operations.Parameters.Any())
                {
                    writer.WritePropertyName("parameters");
                    serializer.Serialize(writer, operations.Parameters);
                }

                if (operations.Servers != null && operations.Servers.Any())
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
                    else
                    {
                        try
                        {
                            var value = (OpenApiOperation)serializer.Deserialize(reader, typeof(OpenApiOperation));
                            operations.Add(propertyName, value);
                        }
                        catch
                        {
                            if (operations.ExtensionData == null)
                            {
                                operations.ExtensionData = new Dictionary<string, object>();
                            }

                            operations.ExtensionData[propertyName] = serializer.Deserialize(reader);
                        }
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