//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    /// <summary>Generates multiple clients and operation names based on the Swagger operation ID (underscore separated).</summary>
    public class MultipleClientsFromOperationIdOperationNameGenerator : IOperationNameGenerator
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
            return GetClientName(operation);
        }

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public virtual string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var clientName = GetClientName(operation);
            var operationName = GetOperationName(operation);

            var hasOperationWithSameName = document.Operations
                .Where(o => o.Operation != operation)
                .Any(o => GetClientName(o.Operation) == clientName && GetOperationName(o.Operation) == operationName);

            if (hasOperationWithSameName)
            {
                if (operationName.ToLowerInvariant().StartsWith("get"))
                {
                    var isArrayResponse = operation.ActualResponses.ContainsKey("200") &&
                                          operation.ActualResponses["200"].ActualResponseSchema != null &&
                                          operation.ActualResponses["200"].ActualResponseSchema.Type.HasFlag(JsonObjectType.Array);

                    if (isArrayResponse)
                        return "GetAll" + operationName.Substring(3);
                }
            }

            return operationName;
        }

        private string GetClientName(SwaggerOperation operation)
        {
            var segments = operation.OperationId.Split('_').Reverse().ToArray();
            return segments.Length >= 2 ? segments[1] : string.Empty;
        }

        private string GetOperationName(SwaggerOperation operation)
        {
            var segments = operation.OperationId.Split('_').Reverse().ToArray();
            return segments.FirstOrDefault() ?? "Index";
        }
    }
}