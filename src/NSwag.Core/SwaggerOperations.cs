//-----------------------------------------------------------------------
// <copyright file="SwaggerOperations.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NSwag.Collections;

namespace NSwag
{
    /// <summary>A Swagger path.</summary>
    [JsonConverter(typeof(SwaggerOperationsJsonConverter))]
    public class SwaggerOperations : ObservableDictionary<SwaggerOperationMethod, SwaggerOperation>
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerOperations"/> class.</summary>
        public SwaggerOperations()
        {
            CollectionChanged += (sender, args) =>
            {
                foreach (var operation in Values)
                    operation.Parent = this;
            };
        }

        /// <summary>Gets the parent <see cref="SwaggerDocument"/>.</summary>
        [JsonIgnore]
        public SwaggerDocument Parent { get; internal set; }

        /// <summary>Gets or sets the parameters.</summary>
        [JsonProperty(PropertyName = "parameters", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<SwaggerParameter> Parameters { get; set; }

        // Needed to convert dictionary keys to lower case
        internal class SwaggerOperationsJsonConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var operations = (SwaggerOperations)value;
                writer.WriteStartObject();

                if (operations.Parameters != null && operations.Parameters.Any())
                {
                    writer.WritePropertyName("parameters");
                    serializer.Serialize(writer, operations.Parameters);
                }

                foreach (var pair in operations)
                {
                    writer.WritePropertyName(pair.Key.ToString().ToLower());
                    serializer.Serialize(writer, pair.Value);
                }
                writer.WriteEndObject();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                var operations = new SwaggerOperations();
                while (reader.Read() && reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();
                    reader.Read();

                    if (propertyName == "parameters")
                        operations.Parameters = (List<SwaggerParameter>)serializer.Deserialize(reader, typeof(List<SwaggerParameter>));
                    else
                    {
                        var key = (SwaggerOperationMethod)Enum.Parse(typeof(SwaggerOperationMethod), propertyName, true);
                        var value = (SwaggerOperation)serializer.Deserialize(reader, typeof(SwaggerOperation));
                        operations.Add(key, value);
                    }

                }
                return operations;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(SwaggerOperations);
            }
        }
    }
}