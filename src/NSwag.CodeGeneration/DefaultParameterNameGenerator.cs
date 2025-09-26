//-----------------------------------------------------------------------
// <copyright file="DefaultParameterNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;

namespace NSwag.CodeGeneration
{
    /// <summary>The default parameter name generator.</summary>
    public sealed class DefaultParameterNameGenerator : IParameterNameGenerator
    {
        private static readonly char[] _parameterNameCleanupChars = ['-', '.', ':', '$', '@', '[', ']'];

#if NET8_0_OR_GREATER
        private static readonly System.Buffers.SearchValues<char> ParameterNameCleanupChars = System.Buffers.SearchValues.Create(_parameterNameCleanupChars);
#else
        private static readonly char[] ParameterNameCleanupChars = _parameterNameCleanupChars;
#endif

        /// <summary>Generates the parameter name for the given parameter.</summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="allParameters">All parameters.</param>
        /// <returns>The parameter name.</returns>
        public string Generate(OpenApiParameter parameter, IEnumerable<OpenApiParameter> allParameters)
        {
            var variableName = GetVariableName(parameter);

            if (allParameters.Count(p => GetVariableName(p) == variableName) > 1)
            {
                return variableName + parameter.Kind;
            }

            return variableName;

            static string GetVariableName(OpenApiParameter openApiParameter)
            {
                var name = !string.IsNullOrEmpty(openApiParameter.OriginalName)
                    ? openApiParameter.OriginalName
                    : openApiParameter.Name;

                if (string.IsNullOrEmpty(name))
                {
                    return "unnamed";
                }

                if (name.AsSpan().IndexOfAny(ParameterNameCleanupChars) != -1)
                {
                    name = name
                        .Replace("-", "_")
                        .Replace(".", "_")
                        .Replace(":", "_")
                        .Replace("$", string.Empty)
                        .Replace("@", string.Empty)
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty);
                }

                return ConversionUtilities.ConvertToLowerCamelCase(name, true);
            }
        }
    }
}