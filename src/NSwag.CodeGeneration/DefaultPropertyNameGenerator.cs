using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration
{
    internal class DefaultPropertyNameGenerator : IPropertyNameGenerator
    {
        /// <inheritdoc />
        public string Generate(JsonProperty property)
        {
            return ConversionUtilities.ConvertToUpperCamelCase(property.Name, true);
        }
    }
}
