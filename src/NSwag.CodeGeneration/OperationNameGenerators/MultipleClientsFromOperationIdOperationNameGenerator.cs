//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;
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
        public virtual string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            return GetClientName(operation).ToString();
        }

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public virtual string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            var clientName = GetClientName(operation);
            var operationName = GetOperationName(operation);

            var hasOperationWithSameName = false;
            // keep iteration logic in sync with OpenApiDocument.Operations - this version is faster as being called a lot
            // Operations property also allocates OpenApiOperationDescription wrapper for each item
            foreach (var p in document._paths)
            {
                foreach (var pair in p.Value.ActualPathItem)
                {
                    var documentOperation = pair.Value;
                    if (documentOperation == operation)
                    {
                        continue;
                    }

                    if (GetOperationName(documentOperation).SequenceEqual(operationName)
                        && GetClientName(documentOperation).SequenceEqual(clientName))
                    {
                        hasOperationWithSameName = true;
                        break;
                    }
                }
            }

            if (hasOperationWithSameName)
            {
                if (operationName.StartsWith("get".AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                {
                    var isArrayResponse = operation.ActualResponses.TryGetValue("200", out var response) &&
                                          response.Schema?.ActualSchema.Type.HasFlag(JsonObjectType.Array) == true;

                    if (isArrayResponse)
                    {
                        return "GetAll" + operationName.Slice(3).ToString();
                    }
                }
            }

            return operationName.ToString();
        }

        private static ReadOnlySpan<char> GetClientName(OpenApiOperation operation)
        {
            ReadOnlySpan<char> operationIdSpan = operation.OperationId.AsSpan();
            const char underscoreSeparator = '_';
            int idxFirst = operationIdSpan.IndexOf(underscoreSeparator);

            // no underscore, fast path
            if (idxFirst == -1)
            {
                return [];
            }

            int idxLast = operationIdSpan.LastIndexOf(underscoreSeparator);

            // only one underscore
            if (idxFirst == idxLast)
            {
                // underscore is the first character
                if (idxFirst == 0)
                {
                    return [];
                }

                return operationIdSpan.Slice(0, idxFirst);
            }

            // backwards search for the second underscore
            // e.g. OperationId_SecondUnderscore_Test => SecondUnderscore
            operationIdSpan = operationIdSpan.Slice(0, idxLast);
            int idxSecondLast = operationIdSpan.LastIndexOf(underscoreSeparator);

            return operationIdSpan.Slice(idxSecondLast + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ReadOnlySpan<char> GetOperationName(OpenApiOperation operation)
        {
            var span = operation.OperationId.AsSpan();
            var idx = span.LastIndexOf('_');
            return idx != -1 && idx < span.Length - 1
                ? span.Slice(idx + 1)
                : span;
        }
    }
}