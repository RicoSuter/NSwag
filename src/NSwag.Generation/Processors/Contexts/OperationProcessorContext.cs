//-----------------------------------------------------------------------
// <copyright file="OperationProcessorContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using NJsonSchema.Generation;

namespace NSwag.Generation.Processors.Contexts
{
    /// <summary>The <see cref="IOperationProcessor"/> context.</summary>
    public class OperationProcessorContext
    {
        /// <summary>Initializes a new instance of the <see cref="OperationProcessorContext" /> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="controllerType">Type of the controller.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="documentGenerator">The OpenAPI generator.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        public OperationProcessorContext(
            OpenApiDocument document,
            OpenApiOperationDescription operationDescription,
            Type controllerType,
            MethodInfo methodInfo,
            OpenApiDocumentGenerator documentGenerator,
            JsonSchemaResolver schemaResolver,
            OpenApiDocumentGeneratorSettings settings,
            IList<OpenApiOperationDescription> allOperationDescriptions)
        {
            Document = document;

            OperationDescription = operationDescription;
            ControllerType = controllerType;
            MethodInfo = methodInfo;

            DocumentGenerator = documentGenerator;
            SchemaGenerator = documentGenerator?.SchemaGenerator;
            SchemaResolver = schemaResolver;

            Settings = settings;
            AllOperationDescriptions = allOperationDescriptions;
        }

        /// <summary>Gets the Swagger document.</summary>
        public OpenApiDocument Document { get; }

        /// <summary>Gets or sets the operation description.</summary>
        public OpenApiOperationDescription OperationDescription { get; }

        /// <summary>Gets the type of the controller.</summary>
        /// <value>The type of the controller.</value>
        public Type ControllerType { get; }

        /// <summary>Gets or sets the method information.</summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>Gets or sets the Swagger generator.</summary>
        public OpenApiDocumentGenerator DocumentGenerator { get; }

        /// <summary>Gets the schema resolver.</summary>
        public JsonSchemaResolver SchemaResolver { get; }

        /// <summary>Gets the settings.</summary>
        public OpenApiDocumentGeneratorSettings Settings { get; }

        /// <summary>Gets the schema generator.</summary>
        public JsonSchemaGenerator SchemaGenerator { get; }

        /// <summary>Gets or sets all operation descriptions.</summary>
        public IList<OpenApiOperationDescription> AllOperationDescriptions { get; }

        /// <summary>Gets the ParameterInfo to SwaggerParameter mappings.</summary>
        public IReadOnlyDictionary<ParameterInfo, OpenApiParameter> Parameters { get; } = new Dictionary<ParameterInfo, OpenApiParameter>();
    }
}