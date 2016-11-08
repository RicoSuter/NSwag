//-----------------------------------------------------------------------
// <copyright file="SingleClientFromOperationIdOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace NSwag.CodeGeneration.CodeGenerators.OperationNameGenerators
{
    /// <summary>Generates the client and operation name based on the Swagger operation ID.</summary>
    public class SingleClientFromOperationIdOperationNameGenerator : IOperationNameGenerator
    {        
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        public bool SupportsMultipleClients { get; } = true;

        /// <summary>Gets the client name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetClientName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return string.Empty;
        }

        /// <summary>Gets the client name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation)
        {
            return operation.OperationId; 
        }
    }
}