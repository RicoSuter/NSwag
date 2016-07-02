//-----------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using NJsonSchema;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Post processes a generated <see cref="SwaggerOperation"/>.</summary>
    public interface IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        bool Process(SwaggerOperationDescription operationDescription, MethodInfo methodInfo, ISchemaResolver schemaResolver, IList<SwaggerOperationDescription> allOperationDescriptions);
    }
}