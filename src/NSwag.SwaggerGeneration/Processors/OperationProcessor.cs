//-----------------------------------------------------------------------
// <copyright file="OperationProcessor.cs" company="NSwag">
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
    /// <summary>A function based operation processor.</summary>
    public class OperationProcessor : IOperationProcessor
    {
        private readonly Func<OperationProcessorContext, Task<bool>> _func;

        /// <summary>Initializes a new instance of the <see cref="OperationProcessor"/> class.</summary>
        /// <param name="func">The processor function.</param>
        public OperationProcessor(Func<OperationProcessorContext, Task<bool>> func)
        {
            _func = func;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            return await _func(context);
        }
    }
}