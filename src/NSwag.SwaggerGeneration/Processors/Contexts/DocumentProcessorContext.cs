//-----------------------------------------------------------------------
// <copyright file="DocumentProcessorContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using NJsonSchema;
using NJsonSchema.Generation;

namespace NSwag.SwaggerGeneration.Processors.Contexts
{
    /// <summary>The <see cref="IDocumentProcessor"/> context.</summary>
    public class DocumentProcessorContext
    {
        /// <summary>Initializes a new instance of the <see cref="DocumentProcessorContext" /> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="controllerTypes">The controller types.</param>
        /// <param name="schemaResolver">The schema resolver.</param>
        /// <param name="schemaGenerator">The schema generator.</param>
        /// <param name="settings">The settings.</param>
        public DocumentProcessorContext(SwaggerDocument document, IEnumerable<Type> controllerTypes, 
            JsonSchemaResolver schemaResolver, JsonSchemaGenerator schemaGenerator, SwaggerGeneratorSettings settings)
        {
            Document = document;
            ControllerTypes = controllerTypes;
            SchemaResolver = schemaResolver;
            SchemaGenerator = schemaGenerator;
            Settings = settings;
        }

        /// <summary>Gets the Swagger document.</summary>
        public SwaggerDocument Document { get; }

        /// <summary>Gets the controller types.</summary>
        public IEnumerable<Type> ControllerTypes { get; }

        /// <summary>Gets or sets the schema resolver.</summary>
        public JsonSchemaResolver SchemaResolver { get; }

        /// <summary>Gets the schema generator (call Generate() with JsonSchemaResolver property!).</summary>
        public JsonSchemaGenerator SchemaGenerator { get; }

        /// <summary>Gets the settings.</summary>
        public SwaggerGeneratorSettings Settings { get; }
    }
}
