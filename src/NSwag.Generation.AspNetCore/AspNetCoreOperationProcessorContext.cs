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
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.AspNetCore
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
            OpenApiDocument document, 
            OpenApiOperationDescription operationDescription, 
            Type controllerType, 
            MethodInfo methodInfo, 
            OpenApiDocumentGenerator swaggerGenerator, 
            JsonSchemaGenerator schemaGenerator, 
            JsonSchemaResolver schemaResolver, 
            OpenApiDocumentGeneratorSettings settings, 
            IList<OpenApiOperationDescription> allOperationDescriptions) 
            : base(document, operationDescription, controllerType, methodInfo, swaggerGenerator, schemaGenerator, schemaResolver, settings, allOperationDescriptions)
        {
        }

        /// <summary>The <see cref="Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription"/>.</summary>
        public ApiDescription ApiDescription { get; set; }
    }
}
