//-----------------------------------------------------------------------
// <copyright file="OperationProcessorContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts
{
    /// <summary>The <see cref="IOperationProcessor"/> context.</summary>
    public class OperationProcessorContext
    {
        /// <summary>Initializes a new instance of the <see cref="OperationProcessorContext"/> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="swaggerGenerator">The swagger generator.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        public OperationProcessorContext(
            SwaggerDocument document,
            SwaggerOperationDescription operationDescription,
            MethodInfo methodInfo,
            SwaggerGenerator swaggerGenerator,
            IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            Document = document;
            OperationDescription = operationDescription;
            MethodInfo = methodInfo;
            SwaggerGenerator = swaggerGenerator;
            AllOperationDescriptions = allOperationDescriptions;
        }

        /// <summary>Gets the Swagger document.</summary>
        public SwaggerDocument Document { get; set; }

        /// <summary>Gets or sets the operation description.</summary>
        public SwaggerOperationDescription OperationDescription { get; set; }

        /// <summary>Gets or sets the method information.</summary>
        public MethodInfo MethodInfo { get; set; }

        /// <summary>Gets or sets the Swagger generator.</summary>
        public SwaggerGenerator SwaggerGenerator { get; set; }

        /// <summary>Gets or sets all operation descriptions.</summary>
        public IList<SwaggerOperationDescription> AllOperationDescriptions { get; set; }
    }
}