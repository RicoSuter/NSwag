//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromPathSegmentsOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    /// <summary>Generates the client and operation name based on the path segments (operation name = last segment, client name = second to last segment).</summary>
    public class SingleClientFromPathSegmentsOperationNameGenerator : IOperationNameGenerator
    {
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        public bool SupportsMultipleClients { get; } = true;

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetClientName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return string.Empty;
        }

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var appendMethod = false;
            var matchingPaths = document.Paths.Where(p => p.Key.Trim('/') == path).ToList();
            if (matchingPaths.Count() == 1)
            {
                var pathOperations = matchingPaths[0].Value.Keys;
                appendMethod = pathOperations.Count > 1;
            }
            else
            {
                // Ambiguous paths?
            }
            var operationName = path
                .Split('/', '-', '_')
                .Where(part => !part.Contains("{") && !string.IsNullOrWhiteSpace(part))
                .Aggregate("", (current, part) => current + CapitalizeFirst(part));
            if (string.IsNullOrEmpty(operationName))
            {
                operationName = "Index";
                appendMethod = true;
            }
            return operationName + (appendMethod ? CapitalizeFirst(httpMethod.ToString()) : "");
        }

        /// <summary>Capitalizes first letter.</summary>
        /// <param name="name">The name to capitalize.</param>
        /// <returns>Capitalized name.</returns>
        internal static string CapitalizeFirst(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }
            var capitalized = name.ToLower();
            return char.ToUpper(capitalized[0]) + (capitalized.Length > 1 ? capitalized.Substring(1) : "");
        }
    }
}