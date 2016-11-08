//-----------------------------------------------------------------------
// <copyright file="IOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics.Contracts;

namespace NSwag.CodeGeneration.CodeGenerators.OperationNameGenerators
{
    /// <summary>Generates the client and operation name for a given operation.</summary>
    public interface IOperationNameGenerator
    {
        /// <summary>Gets a value indicating whether the generator supports multiple client classes.</summary>
        bool SupportsMultipleClients { get; }

        /// <summary>Gets the client name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        [Pure]
        string GetClientName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation);

        /// <summary>Gets the operation name for a given operation.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The operation name.</returns>
        [Pure]
        string GetOperationName(SwaggerDocument document, string path, SwaggerOperationMethod httpMethod, SwaggerOperation operation);
    }
}