//-----------------------------------------------------------------------
// <copyright file="OperationConsumesProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    /// <summary>Generates the consumes clause from the operation's ConsumesAttribute.</summary>
    public class OperationConsumesProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            // Check if the action method itself has the Consumes Attribute
            dynamic consumesAttribute = context.MethodInfo
                .GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "ConsumesAttribute");

            if (consumesAttribute == null)
            {
                // If the action method does not have a Consumes Attribute we'll try with its containing class
                consumesAttribute = context.MethodInfo.DeclaringType
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .SingleOrDefault(a => a.GetType().Name == "ConsumesAttribute");
            }

            if (consumesAttribute != null && consumesAttribute.ContentTypes != null)
            {
                if (context.OperationDescription.Operation.Consumes == null)
                    context.OperationDescription.Operation.Consumes = new List<string>(consumesAttribute.ContentTypes);
                else
                    context.OperationDescription.Operation.Consumes.AddRange(consumesAttribute.ContentTypes);
            }

            return Task.FromResult(true);
        }
    }
}
