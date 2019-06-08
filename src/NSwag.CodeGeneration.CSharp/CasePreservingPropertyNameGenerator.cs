using NJsonSchema;
using NJsonSchema.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>Generates the property name for a given CSharp <see cref="JsonSchemaProperty"/>, preserving the case of the first letter.</summary>
    class CasePreservingPropertyNameGenerator : IPropertyNameGenerator
    {
        /// <summary>Generates the property name.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The new name.</returns>
        public virtual string Generate(JsonSchemaProperty property)
        {
            return property.Name
                    .Replace("\"", string.Empty)
                    .Replace("@", string.Empty)
                    .Replace("$", string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Replace(".", "-")
                    .Replace("=", "-")
                    .Replace("+", "plus")
                    .Replace("*", "Star")
                    .Replace(":", "_")
                    .Replace("-", "_");
        }
    }
}
