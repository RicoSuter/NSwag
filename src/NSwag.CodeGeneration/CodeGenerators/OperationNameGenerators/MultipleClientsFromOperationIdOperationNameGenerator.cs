//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using NJsonSchema;

namespace NSwag.CodeGeneration.CodeGenerators.OperationNameGenerators
{
    /// <summary>Generates multiple clients and operation names based on the Swagger operation ID (underscore separated).</summary>
    public class MultipleClientsFromOperationIdOperationNameGenerator : IOperationNameGenerator
    {
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        public bool SupportsMultipleClients { get; } = true;

        /// <summary>Gets the client name for a given operation.</summary>
        /// <param name="service">The Swagger service.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetClientName(SwaggerService service, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return GetClientName(operation);
        }

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="service">The Swagger service.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public string GetOperationName(SwaggerService service, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var clientName = GetClientName(operation);
            var operationName = GetOperationName(operation);

            var hasOperationWithSameName = service.Operations
                .Where(o => o.Operation != operation)
                .Any(o => GetClientName(o.Operation) == clientName && GetOperationName(o.Operation) == operationName);

            if (hasOperationWithSameName)
            {
                if (operationName.ToLowerInvariant().StartsWith("get"))
                {
                    var isArrayResponse = operation.Responses.ContainsKey("200") &&
                                          operation.Responses["200"].Schema != null &&
                                          operation.Responses["200"].Schema.Type.HasFlag(JsonObjectType.Array);

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
            return segments.First();
        }
    }
}