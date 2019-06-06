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
using Namotion.Reflection;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
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
            {
                AddControllerNameTag(context);
            }

            return true;
        }

        /// <summary>Adds the controller name as operation tag.</summary>
        /// <param name="context">The context.</param>
        protected virtual void AddControllerNameTag(OperationProcessorContext context)
        {
            var controllerName = context.ControllerType.Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            }

            context.OperationDescription.Operation.Tags.Add(controllerName);
        }

        private void ProcessSwaggerTagAttributes(OpenApiDocument document, OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            foreach (var tagAttribute in methodInfo.GetCustomAttributes()
                .GetAssignableToTypeName("SwaggerTagAttribute", TypeNameStyle.Name)
                .Select(a => (dynamic)a))
            {
                if (operationDescription.Operation.Tags.All(t => t != tagAttribute.Name))
                {
                    operationDescription.Operation.Tags.Add(tagAttribute.Name);
                }

                if (ObjectExtensions.HasProperty(tagAttribute, "AddToDocument") && tagAttribute.AddToDocument)
                {
                    DocumentTagsProcessor.ProcessTagAttribute(document, tagAttribute);
                }
            }
        }

        private void ProcessSwaggerTagsAttribute(OpenApiDocument document, OpenApiOperationDescription operationDescription, MethodInfo methodInfo)
        {
            dynamic tagsAttribute = methodInfo
                .GetCustomAttributes()
                .FirstAssignableToTypeNameOrDefault("SwaggerTagsAttribute", TypeNameStyle.Name);

            if (tagsAttribute != null)
            {
                var tags = ((string[])tagsAttribute.Tags).ToList();
                foreach (var tag in tags)
                {
                    if (operationDescription.Operation.Tags.All(t => t != tag))
                    {
                        operationDescription.Operation.Tags.Add(tag);
                    }

                    if (ObjectExtensions.HasProperty(tagsAttribute, "AddToDocument") && tagsAttribute.AddToDocument)
                    {
                        if (document.Tags == null)
                        {
                            document.Tags = new List<OpenApiTag>();
                        }

                        if (document.Tags.All(t => t.Name != tag))
                        {
                            document.Tags.Add(new OpenApiTag { Name = tag });
                        }
                    }
                }
            }
        }
    }
}