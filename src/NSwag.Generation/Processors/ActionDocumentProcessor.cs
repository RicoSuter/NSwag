//-----------------------------------------------------------------------
// <copyright file="ActionDocumentProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
{
    /// <summary>A generic action/function based document processor.</summary>
    public class ActionDocumentProcessor : IDocumentProcessor
    {
        private readonly Action<DocumentProcessorContext> _action;
        /// <summary>Initializes a new instance of the <see cref="ActionDocumentProcessor"/> class.</summary>
        public ActionDocumentProcessor(Action<DocumentProcessorContext> action)
        {
            _action = action;
        }

        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="context">The processor context.</param>
        public void Process(DocumentProcessorContext context)
        {
            _action(context);
        }
    }
}