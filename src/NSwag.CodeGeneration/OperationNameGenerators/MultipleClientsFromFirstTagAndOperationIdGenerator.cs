//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromFirstTagAndOperationIdGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using System.Linq;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    /// <summary>Generates the client name based on the first tag and operation name based on the operation id (operation name = operationId, client name = first tag).</summary>
    public class MultipleClientsFromFirstTagAndOperationIdGenerator : IOperationNameGenerator
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
            return ConversionUtilities.ConvertToUpperCamelCase(operation.Tags.FirstOrDefault(), false);
        }

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        public virtual string GetOperationName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            var operationName = operation.OperationId;
            return operationName;
        }
    }
}
