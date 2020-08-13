//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using DotLiquid;
using NJsonSchema;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    /// <summary>Generates a single client and operation names based on the method name (underscore separated).</summary>
    public class SingleClientFromActionOperationNameGenerator : IOperationNameGenerator
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

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public virtual string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            var operationName = GetOperationName(operation);

            var hasOperationWithSameName = HasOperationWithSameName(document, operation, operationName);

            if (hasOperationWithSameName)
            {
                operationName = GetOperationNameFromPath(document, path, httpMethod, operation);
            }

            return operationName;
        }

        private bool HasOperationWithSameName(OpenApiDocument document, OpenApiOperation operation, string operationName)
        {
            return document.Operations
                .Where(o => o.Operation != operation)
                .Any(o => GetOperationName(o.Operation) == operationName);
        }

        private string GetOperationName(OpenApiOperation operation)
        {
            var segments = operation.OperationId.Split('_').Reverse().ToArray();
            return segments.FirstOrDefault() ?? "Index";
        }

        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        private string GetOperationNameFromPath(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
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
                operationName += CapitalizeFirst(httpMethod);
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

            var capitalized = name.ToLowerInvariant();
            return char.ToUpperInvariant(capitalized[0]) + (capitalized.Length > 1 ? capitalized.Substring(1) : "");
        }
    }
}