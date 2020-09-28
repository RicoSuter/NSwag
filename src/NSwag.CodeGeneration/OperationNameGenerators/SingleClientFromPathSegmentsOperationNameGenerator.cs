//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromPathSegmentsOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Text.RegularExpressions;

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
        public virtual string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            return string.Empty;
        }

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public virtual string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            var operationName = ConvertPathToName(path);
            var hasNameConflict = document.Paths
                .SelectMany(pair => pair.Value.ActualPathItem
                    .Select(p => new { Path = pair.Key.Trim('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Where(op => 
                    GetClientName(document, op.Path, op.HttpMethod, op.Operation) == GetClientName(document, path, httpMethod, operation) && 
                    ConvertPathToName(op.Path) == operationName
                )
                .ToList().Count > 1;

            if (hasNameConflict)
            {
                operationName += CapitalizeFirst(httpMethod);
            }

            return operationName;
        }

        /// <summary>Converts the path to an operation name.</summary>
        /// <param name="path">The HTTP path.</param>
        /// <returns>The operation name.</returns>
        public static string ConvertPathToName(string path)
        {
            var name = Regex.Replace(path, @"\{.*?\}", "")
                .Split('/', '-', '_')
                .Where(part => !part.Contains("{") && !string.IsNullOrWhiteSpace(part))
                .Aggregate("", (current, part) => current + CapitalizeFirst(part));

            if (string.IsNullOrEmpty(name))
            {
                name = "Index"; // Root path based operation?
            }

            return name;
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

            var capitalized = name.ToLowerInvariant();
            return char.ToUpperInvariant(capitalized[0]) + (capitalized.Length > 1 ? capitalized.Substring(1) : "");
        }
    }
}