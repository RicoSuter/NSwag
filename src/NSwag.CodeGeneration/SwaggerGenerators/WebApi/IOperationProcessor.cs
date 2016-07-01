//-----------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Reflection;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Post processes a generated <see cref="SwaggerOperation"/>.</summary>
    public interface IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="path">The path.</param>
        /// <param name="method">The method.</param>
        /// <param name="operation">The operation to process.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        bool Process(MethodInfo methodInfo, string path, SwaggerOperationMethod method, SwaggerOperation operation);
    }
}