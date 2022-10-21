//-----------------------------------------------------------------------
// <copyright file="OperationTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Namotion.Reflection;
using NSwag.Generation.Collections;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
{
    /// <summary>Processes the SwaggerTagsAttribute on the operation method.</summary>
    public class OperationTagsProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public virtual bool Process(OperationProcessorContext context)
        {
            if (context.MethodInfo != null)
            {
                ProcessSwaggerTagsAttribute(context.Document, context.OperationDescription, context.MethodInfo);
                ProcessSwaggerTagAttributes(context.Document, context.OperationDescription, context.MethodInfo);
            }

            if (context.ControllerType != null)
            {
                if (!context.OperationDescription.Operation.Tags.Any())
                {
                    var typeInfo = context.ControllerType.GetTypeInfo();

                    ProcessControllerSwaggerTagsAttribute(context.OperationDescription, typeInfo);
                    ProcessControllerSwaggerTagAttributes(context.OperationDescription, typeInfo);
                }

                if (!context.OperationDescription.Operation.Tags.Any())
                {
                    AddControllerNameTag(context);
                }
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

            var summary = context.ControllerType.GetXmlDocsSummary(context.Settings.GetXmlDocsOptions());
            context.OperationDescription.Operation.Tags.Add(controllerName);
            UpdateDocumentTagDescription(context, controllerName, summary);
        }

        /// <summary>
        /// Sets the description for the given controller on the document.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tagName">The tag name.</param>
        /// <param name="description">The description.</param>
        protected void UpdateDocumentTagDescription(OperationProcessorContext context, string tagName, string description)
        {
            if (!context.Settings.UseControllerSummaryAsTagDescription || string.IsNullOrEmpty(description))
            {
                return;
            }

            var documentTag = context.Document.Tags.SingleOrNew(tag => tag.Name == tagName);
            documentTag.Name = tagName;
            documentTag.Description = description;
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

        private void ProcessControllerSwaggerTagsAttribute(OpenApiOperationDescription operationDescription, TypeInfo typeInfo)
        {
            dynamic tagsAttribute = typeInfo
                .GetCustomAttributes()
                .FirstAssignableToTypeNameOrDefault("OpenApiTagsAttribute", TypeNameStyle.Name);

            if (tagsAttribute != null)
            {
                var tags = ((string[])tagsAttribute.Tags).ToList();
                foreach (var tag in tags)
                {
                    if (operationDescription.Operation.Tags.All(t => t != tag))
                    {
                        operationDescription.Operation.Tags.Add(tag);
                    }
                }
            }
        }

        private void ProcessControllerSwaggerTagAttributes(OpenApiOperationDescription operationDescription, TypeInfo typeInfo)
        {
            foreach (var tagAttribute in typeInfo.GetCustomAttributes()
                .GetAssignableToTypeName("OpenApiTagAttribute", TypeNameStyle.Name)
                .Select(a => (dynamic)a))
            {
                if (operationDescription.Operation.Tags.All(t => t != tagAttribute.Name))
                {
                    operationDescription.Operation.Tags.Add(tagAttribute.Name);
                }
            }
        }
    }
}