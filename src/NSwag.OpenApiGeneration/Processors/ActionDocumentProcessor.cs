//-----------------------------------------------------------------------
// <copyright file="ActionDocumentProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.Processors
{
    /// <summary>A generic action/function based document processor.</summary>
    public class ActionDocumentProcessor : IDocumentProcessor
    {
        private readonly Func<DocumentProcessorContext, Task> _action;

        /// <summary>Initializes a new instance of the <see cref="ActionDocumentProcessor"/> class.</summary>
        public ActionDocumentProcessor(Action<DocumentProcessorContext> action)
        {
            _action = (context) =>
            {
                action(context);
                return Task.FromResult<object>(null);
            };
        }

        /// <summary>Initializes a new instance of the <see cref="ActionDocumentProcessor"/> class.</summary>
        public ActionDocumentProcessor(Func<DocumentProcessorContext, Task> action)
        {
            _action = action;
        }

        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="context">The processor context.</param>
        public async Task ProcessAsync(DocumentProcessorContext context)
        {
            await _action(context);
        }
    }
}