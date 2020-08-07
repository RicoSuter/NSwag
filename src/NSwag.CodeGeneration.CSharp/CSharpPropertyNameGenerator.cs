//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
//     Copyright (c) AMain.com. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
// <author>Kendall Bennett, kendallb@amain.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using NJsonSchema.CodeGeneration;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>
    /// Custom property name generator to not have underscores in the name
    /// </summary>
    /// <summary>Generates the property name for a given CSharp <see cref="JsonSchemaProperty"/>.</summary>
    public sealed class CSharpPropertyNameGenerator : IPropertyNameGenerator
    {
        /// <summary>Generates the property name.</summary>
        /// <param name="property">The property.</param>
        /// <returns>The new name.</returns>
        public string Generate(JsonSchemaProperty property)
        {
            return ConversionUtilities.ConvertToUpperCamelCase(property.Name
                    .Replace("\"", string.Empty)
                    .Replace("@", string.Empty)
                    .Replace("?", string.Empty)
                    .Replace("$", string.Empty)
                    .Replace("[", string.Empty)
                    .Replace("]", string.Empty)
                    .Replace(".", "-")
                    .Replace("=", "-")
                    .Replace("+", "plus")
                    .Replace("_", "-"), true)
                .Replace("*", "Star")
                .Replace(":", "_")
                .Replace("-", "_")
                .Replace("#", "_");
        }
    }
}