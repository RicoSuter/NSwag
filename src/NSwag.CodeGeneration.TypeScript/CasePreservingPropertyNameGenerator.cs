using NJsonSchema;
using NJsonSchema.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSwag.CodeGeneration.TypeScript
{
    /// <summary>Generates the property name for a given TypeScript <see cref="JsonSchemaProperty"/>, preserving the case of the first letter.</summary>
    class CasePreservingPropertyNameGenerator : IPropertyNameGenerator
    {
        /// <summary>Gets or sets the reserved names.</summary>
        public IEnumerable<string> ReservedPropertyNames { get; set; } = new List<string> { "constructor" };

        /// <summary>Generates the property name.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The new name.</returns>
        public virtual string Generate(JsonSchemaProperty property)
        {
            var name = property.Name
                    .Replace("\"", string.Empty)
                    .Replace("@", string.Empty)
                    .Replace(".", "-")
                    .Replace("=", "-")
                    .Replace("+", "plus")
                    .Replace("*", "Star")
                    .Replace(":", "_")
                    .Replace("-", "_");

            if (ReservedPropertyNames.Contains(name))
            {
                return name + "_";
            }

            return name;
        }
    }
}
