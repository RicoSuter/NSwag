//-----------------------------------------------------------------------
// <copyright file="SwaggerPathItem.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
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
    [JsonConverter(typeof(SwaggerPathItemConverter))]
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

        // Needed to convert dictionary keys to lower case
        internal class SwaggerPathItemConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var operations = (OpenApiPathItem)value;
                writer.WriteStartObject();

                if (operations.Parameters != null && operations.Parameters.Any())
                {
                    writer.WritePropertyName("parameters");
                    serializer.Serialize(writer, operations.Parameters);
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

                    if (propertyName == "parameters")
                    {
                        operations.Parameters = (List<OpenApiParameter>)serializer.Deserialize(reader, typeof(List<OpenApiParameter>));
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