//-----------------------------------------------------------------------
// <copyright file="OperationProcessorCollection.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>The <see cref="IOperationProcessor"/> context that use <see cref="Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription"/>.</summary>
    public class AspNetCoreOperationProcessorContext : OperationProcessorContext
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreOperationProcessorContext" /> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="controllerType">Type of the controller.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="swaggerGenerator">The swagger generator.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="settings">The sett</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        public AspNetCoreOperationProcessorContext(
            SwaggerDocument document, 
            SwaggerOperationDescription operationDescription, 
            Type controllerType, 
            MethodInfo methodInfo, 
            SwaggerGenerator swaggerGenerator, 
            JsonSchemaGenerator schemaGenerator, 
            JsonSchemaResolver schemaResolver, 
            SwaggerGeneratorSettings settings, 
            IList<SwaggerOperationDescription> allOperationDescriptions) 
            : base(document, operationDescription, controllerType, methodInfo, swaggerGenerator, schemaGenerator, schemaResolver, settings, allOperationDescriptions)
        {
        }

        /// <summary>The <see cref="Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription"/>.</summary>
        public ApiDescription ApiDescription { get; set; }
    }
}
