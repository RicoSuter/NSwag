//-----------------------------------------------------------------------
// <copyright file="OpenApiParameterJsonConverter.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NSwag.Converters
{
    /// <summary>Custom JSON converter for OpenApiParameter that resolves the property name
    /// collision between OpenApiParameter.IsRequired (bool) and the inherited
    /// JsonSchema.RequiredPropertiesRaw (string[]), which both map to "required" in JSON.
    /// <para>
    /// In OpenAPI, "required" on a parameter is always a boolean. In JSON Schema, "required"
    /// is an array of property names. This converter inspects the JSON value type and routes
    /// it to the correct property.
    /// </para></summary>
    internal class OpenApiParameterJsonConverter : JsonConverter<OpenApiParameter>
    {
        public override OpenApiParameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonNode.Parse(ref reader);
            if (node is not JsonObject obj)
            {
                return null;
            }

            // Extract the boolean "required" before deserialization.
            // If "required" is a bool, it's the parameter's IsRequired property.
            // If "required" is an array, leave it for JsonSchema.RequiredPropertiesRaw.
            bool? isRequired = null;
            if (obj.TryGetPropertyValue("required", out var requiredNode))
            {
                if (requiredNode is JsonValue jsonValue && jsonValue.TryGetValue<bool>(out var boolValue))
                {
                    isRequired = boolValue;
                    obj.Remove("required");
                }
                // If it's an array, leave it — RequiredPropertiesRaw will handle it
            }

            // Deserialize without this converter to avoid recursion
            var optionsWithout = GetOrCreateOptionsWithout(options);
            var parameter = obj.Deserialize<OpenApiParameter>(optionsWithout);

            if (parameter != null && isRequired.HasValue)
            {
                parameter.IsRequired = isRequired.Value;
            }

            return parameter;
        }

        public override void Write(Utf8JsonWriter writer, OpenApiParameter value, JsonSerializerOptions options)
        {
            // Delegate to stripped options for the base serialization (the
            // SchemaSerializationConverter in the full options handles property filtering
            // for OpenApiParameter via its property-by-property Write method, but this
            // attribute converter takes precedence, so we serialize without it and manually
            // add the "required" boolean).
            var optionsWithout = GetOrCreateOptionsWithout(options);
            var node = JsonSerializer.SerializeToNode(value, optionsWithout);

            if (node is JsonObject obj)
            {
                // Add the "required" boolean (IsRequired is [JsonIgnore], so it's not in the node)
                if (value.IsRequired)
                {
                    // Insert "required" after "in" for consistent ordering, or just add it
                    obj["required"] = true;
                }
            }

            node?.WriteTo(writer);
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<JsonSerializerOptions, JsonSerializerOptions> _cache = new();

        private static JsonSerializerOptions GetOrCreateOptionsWithout(JsonSerializerOptions options)
        {
            return _cache.GetOrAdd(options, static parentOptions =>
            {
                var newOptions = new JsonSerializerOptions(parentOptions);
                // Remove the OpenApiParameterJsonConverter from converters list
                for (var i = newOptions.Converters.Count - 1; i >= 0; i--)
                {
                    if (newOptions.Converters[i] is OpenApiParameterJsonConverter)
                    {
                        newOptions.Converters.RemoveAt(i);
                    }
                }
                return newOptions;
            });
        }
    }
}
