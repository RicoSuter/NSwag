using NJsonSchema;

namespace NSwag.CodeGeneration.Models
{
    /// <summary>
    /// A model representing the property or field of a structure or class
    /// </summary>
    public class PropertyModel
    {
        /// <summary>
        /// The key under which the <see cref="JsonProperty"/> is located in the parent JSON object
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The <see cref="JsonProperty"/> represented by this <see cref="PropertyModel"/>
        /// </summary>
        public JsonProperty JsonProperty { get; }

        /// <summary>
        /// The generated property name suitable for use in the target language
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Indicates whether the property is a collection
        /// </summary>
        public bool IsCollection => JsonProperty.Type == JsonObjectType.Array;

        /// <inheritdoc />
        public PropertyModel(string key, JsonProperty jsonProperty, string name)
        {
            Key = key;
            JsonProperty = jsonProperty;
            Name = name;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Name;
        }
    }
}
