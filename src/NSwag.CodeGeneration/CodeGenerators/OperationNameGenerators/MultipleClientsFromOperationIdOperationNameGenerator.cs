//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.OperationNameGenerators
{
    /// <summary>Generates multiple clients and operation names based on the Swagger operation ID (underscore separated).</summary>
    public class MultipleClientsFromOperationIdOperationNameGenerator : IOperationNameGenerator
    {
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        public bool SupportsMultipleClients { get; } = true;

        /// <summary>Gets the client name for a given operation.</summary>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetClientName(string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var segments = operation.OperationId.Split('_');
            return segments.Length >= 2 ? segments[0] : "Anonymous";
        }

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public string GetOperationName(string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            var segments = operation.OperationId.Split('_');
            return segments.Length >= 2 ? segments[1] : "Anonymous";
        }
    }
}