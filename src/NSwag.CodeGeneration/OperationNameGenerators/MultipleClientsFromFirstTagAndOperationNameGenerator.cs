//-----------------------------------------------------------------------
// <copyright file="MultipleClientsFromFirstTagAndOperationNameGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NJsonSchema;
using System.Linq;

namespace NSwag.CodeGeneration.OperationNameGenerators
{
    /// <summary>Generates the client name based on the first tag and operation names based on the Swagger operation ID (underscore separated).</summary>
    public class MultipleClientsFromFirstTagAndOperationNameGenerator : MultipleClientsFromOperationIdOperationNameGenerator, IOperationNameGenerator
    {
        /// <summary>Gets the client name for a given operation (may be empty).</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="path">The HTTP path.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>The client name.</returns>
        public override string GetClientName(OpenApiDocument document, string path, string httpMethod, OpenApiOperation operation)
        {
            return ConversionUtilities.ConvertToUpperCamelCase(operation.Tags.FirstOrDefault(), false);
        }
    }
}
