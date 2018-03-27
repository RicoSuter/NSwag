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
    public class MultipleClientsFromPathSegmentsOperationNameGenerator : IOperationNameGenerator
    {
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        public bool SupportsMultipleClients { get; } = true;

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public virtual string GetClientName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return path
                .Split('/')
                .Where(p => !p.Contains("{") && !string.IsNullOrWhiteSpace(p))
                .Reverse()
                .Skip(1)
                .FirstOrDefault() ?? string.Empty;
        }

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public virtual string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var operationName = ConvertPathToName(path);

            var hasNameConflict = document.Paths
                .SelectMany(pair => pair.Value.Select(p => new { Path = pair.Key.Trim('/'), HttpMethod = p.Key, Operation = p.Value }))
                .Where(op => 
                    GetClientName(document, op.Path, op.HttpMethod, op.Operation) == GetClientName(document, path, httpMethod, operation) && 
                    ConvertPathToName(op.Path) == operationName
                ).ToList()
                .Count > 1;

            if (hasNameConflict)
            {
                operationName += CapitalizeFirst(httpMethod.ToString());
            }

            return operationName;
        }

        /// <summary>Converts the path to an operation name.</summary>
        /// <param name="path">The HTTP path.</param>
        /// <returns>The operation name.</returns>
        internal static string ConvertPathToName(string path)
        {
            return path
                .Split('/')
                .Where(p => !p.Contains("{") && !string.IsNullOrWhiteSpace(p))
                .Reverse()
                .FirstOrDefault() ?? "Index";
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