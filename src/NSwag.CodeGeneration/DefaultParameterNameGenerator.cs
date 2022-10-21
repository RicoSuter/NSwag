//-----------------------------------------------------------------------
// <copyright file="DefaultParameterNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration
{
    /// <summary>The default parameter name generator.</summary>
    public class DefaultParameterNameGenerator : IParameterNameGenerator
    {
        /// <summary>Generates the parameter name for the given parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <returns>The parameter name.</returns>
        public string Generate(OpenApiParameter parameter, IEnumerable<OpenApiParameter> allParameters)
        {
            var name = !string.IsNullOrEmpty(parameter.OriginalName) ? 
                parameter.OriginalName : parameter.Name;

            if (string.IsNullOrEmpty(name))
            {
                return "unnamed";
            }

            var variableName = ConversionUtilities.ConvertToLowerCamelCase(name
                .Replace("-", "_")
                .Replace(".", "_")
                .Replace("$", string.Empty)
                .Replace("@", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty), true);

            if (allParameters.Count(p => p.Name == name) > 1)
            {
                return variableName + parameter.Kind;
            }

            return variableName;
        }
    }
}