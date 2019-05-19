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
using Namotion.Reflection;
using NJsonSchema.Infrastructure;
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
            var operation = context.OperationDescription.Operation;
            if (operation.ExtensionData == null)
            {
                operation.ExtensionData = new Dictionary<string, object>();
            }

            foreach (var extensionDataAttribute in
                    from extensionDataAttribute
                    in context.MethodInfo.GetCustomAttributes()
                        .Where(a => a.GetType().IsAssignableToTypeName("SwaggerExtensionDataAttribute", TypeNameStyle.Name))
                    select (dynamic)extensionDataAttribute)
            {
                string key = extensionDataAttribute.Key;
                string value = extensionDataAttribute.Value;

                operation.ExtensionData[key] = value;
            }

            foreach (var parameter in context.Parameters)
            {
                if (parameter.Value.ExtensionData == null)
                {
                    parameter.Value.ExtensionData = new Dictionary<string, object>();
                }

                foreach (var extensionDataAttribute in
                    from extensionDataAttribute
                    in parameter.Key.GetCustomAttributes(true)
                        .Where(a => a.GetType().IsAssignableToTypeName("SwaggerExtensionDataAttribute", TypeNameStyle.Name))
                    select (dynamic)extensionDataAttribute)
                {
                    string key = extensionDataAttribute.Key;
                    string value = extensionDataAttribute.Value;

                    parameter.Value.ExtensionData[key] = value;
                }
            }

            return Task.FromResult(true);
        }
    }
}