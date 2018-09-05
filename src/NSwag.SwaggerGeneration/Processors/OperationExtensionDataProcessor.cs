//-----------------------------------------------------------------------
// <copyright file="OperationTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.Processors
{
    /// <summary>Processes the SwaggerExtensionDataAttribute on the operation method.</summary>
    public class OperationExtensionDataProcessor : IOperationProcessor
    {
        /// <summary>
        /// Processes the specified method information.
        /// </summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            foreach (var extensionDataAttribute in from extensionDataAttribute in context.MethodInfo.GetCustomAttributes()
                                                                                  .Where(a => a.GetType().Name == "SwaggerExtensionDataAttribute")
                                                   select (dynamic)extensionDataAttribute)
            {
                string key = extensionDataAttribute.Key;
                string value = extensionDataAttribute.Value;

                if (context.OperationDescription.Operation.ExtensionData == null)
                {
                    context.OperationDescription.Operation.ExtensionData = new Dictionary<string, object>();
                }

                context.OperationDescription.Operation.ExtensionData[key] = value;
            }

            // TODO: process for parameters

            return Task.FromResult(true);
        }
    }
}