//-----------------------------------------------------------------------
// <copyright file="DocumentProcessorContext.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts
{
    /// <summary>The <see cref="IDocumentProcessor"/> context.</summary>
    public class DocumentProcessorContext
    {
        /// <summary>Initializes a new instance of the <see cref="DocumentProcessorContext"/> class.</summary>
        /// <param name="document">The document.</param>
        /// <param name="controllerTypes">The controller types.</param>
        public DocumentProcessorContext(SwaggerDocument document, IEnumerable<Type> controllerTypes)
        {
            Document = document;
            ControllerTypes = controllerTypes;
        }

        /// <summary>Gets the Swagger document.</summary>
        public SwaggerDocument Document { get; }

        /// <summary>Gets the controller types.</summary>
        public IEnumerable<Type> ControllerTypes { get; }
    }
}
