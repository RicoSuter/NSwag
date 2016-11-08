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
using NSwag.CodeGeneration.Infrastructure;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Processes the SwaggerTagsAttribute on the operation method.</summary>
    public class OperationTagsProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            ProcessSwaggerTagsAttribute(context.Document, context.OperationDescription, context.MethodInfo);
            ProcessSwaggerTagAttributes(context.Document, context.OperationDescription, context.MethodInfo);

            if (!context.OperationDescription.Operation.Tags.Any())
                context.OperationDescription.Operation.Tags.Add(context.MethodInfo.DeclaringType.Name);

            return true;
        }

        private void ProcessSwaggerTagAttributes(SwaggerDocument document, SwaggerOperationDescription operationDescription, MethodInfo methodInfo)
        {
            var tagAttributes = methodInfo.GetCustomAttributes()
                .Where(a => a.GetType().Name == "SwaggerTagAttribute")
                .Select(a => (dynamic)a)
                .ToArray();

            if (tagAttributes.Any())
            {
                foreach (var tagAttribute in tagAttributes)
                {
                    if (operationDescription.Operation.Tags.All(t => t != tagAttribute.Name))
                        operationDescription.Operation.Tags.Add(tagAttribute.Name);

                    if (ObjectExtensions.HasProperty(tagAttribute, "AddToDocument") && tagAttribute.AddToDocument)
                        DocumentTagsProcessor.AddTagFromSwaggerTagAttribute(document, tagAttribute);
                }
            }
        }

        private void ProcessSwaggerTagsAttribute(SwaggerDocument document, SwaggerOperationDescription operationDescription, MethodInfo methodInfo)
        {
            dynamic tagsAttribute = methodInfo
                .GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "SwaggerTagsAttribute");

            if (tagsAttribute != null)
            {
                var tags = ((string[])tagsAttribute.Tags).ToList();
                foreach (var tag in tags)
                {
                    if (operationDescription.Operation.Tags.All(t => t != tag))
                        operationDescription.Operation.Tags.Add(tag);

                    if (ObjectExtensions.HasProperty(tagsAttribute, "AddToDocument") && tagsAttribute.AddToDocument)
                    {
                        if (document.Tags == null)
                            document.Tags = new List<SwaggerTag>();

                        if (document.Tags.All(t => t.Name != tag))
                            document.Tags.Add(new SwaggerTag { Name = tag });
                    }
                }
            }
        }
    }
}