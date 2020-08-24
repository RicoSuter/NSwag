//-----------------------------------------------------------------------
// <copyright file="SwaggerToCSharpGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
//     Copyright (c) AMain.com. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
// <author>Kendall Bennett, kendallb@amain.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration.CSharp
{
    /// <summary>
    /// Custom type name generator to not have underscores in the name
    /// </summary>
    public class CSharpTypeNameGenerator : DefaultTypeNameGenerator
    {
        /// <summary>Generates the type name for the given schema.</summary>
        /// <param name="schema">The schema.</param>
        /// <param name="typeNameHint">The type name hint.</param>
        /// <returns>The type name.</returns>
        protected override string Generate(JsonSchema schema, string typeNameHint)
        {
            if (string.IsNullOrEmpty(typeNameHint) && schema.HasTypeNameTitle)
                typeNameHint = schema.Title;
            var input = typeNameHint?.Split('.').Last() ?? "Anonymous";
            return ConversionUtilities.ConvertToUpperCamelCase(input.Replace('_', '-'), true);
        }
    }
}