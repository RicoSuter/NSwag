//-----------------------------------------------------------------------
// <copyright file="DocumentTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Namotion.Reflection;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
{
    /// <summary>Processes the SwaggerTagAttribute and SwaggerTagsAttribute on the controller classes.</summary>
    public class DocumentTagsProcessor : IDocumentProcessor
    {
        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="context"></param>
        public void Process(DocumentProcessorContext context)
        {
            foreach (var controllerType in context.ControllerTypes)
            {
                ProcessTagsAttribute(context.Document, controllerType);
                ProcessTagAttributes(context.Document, controllerType);
            }
        }

        private static void ProcessTagsAttribute(OpenApiDocument document, Type controllerType)
        {
            dynamic tagsAttribute = controllerType.ToCachedType()
                .TypeAttributes
                .FirstAssignableToTypeNameOrDefault("SwaggerTagsAttribute", TypeNameStyle.Name);
            if (tagsAttribute != null)
            {
                var tags = ((string[])tagsAttribute.Tags)
                    .Select(t => new OpenApiTag { Name = t })
                    .ToList();

                if (tags.Any())
                {
                    if (document.Tags == null)
                    {
                        document.Tags = new List<OpenApiTag>();
                    }

                    foreach (var tag in tags)
                    {
                        if (document.Tags.All(t => t.Name != tag.Name))
                        {
                            document.Tags.Add(tag);
                        }
                    }
                }
            }
        }

        private static void ProcessTagAttributes(OpenApiDocument document, Type controllerType)
        {
            var tagAttributes = controllerType
                .ToCachedType().TypeAttributes
                .GetAssignableToTypeName("SwaggerTagAttribute", TypeNameStyle.Name)
                .Select(a => (dynamic)a)
                .ToArray();

            if (tagAttributes.Any())
            {
                foreach (var tagAttribute in tagAttributes)
                {
                    ProcessTagAttribute(document, tagAttribute);
                }
            }
        }

        internal static void ProcessTagAttribute(OpenApiDocument document, dynamic tagAttribute)
        {
            if (document.Tags == null)
            {
                document.Tags = new List<OpenApiTag>();
            }

            var tag = document.Tags.SingleOrDefault(t => t.Name == tagAttribute.Name);
            if (tag == null)
            {
                tag = new OpenApiTag();
                document.Tags.Add(tag);
            }

            tag.Description = tagAttribute.Description;
            tag.Name = tagAttribute.Name;

            if (!string.IsNullOrEmpty(tagAttribute.DocumentationDescription) ||
                !string.IsNullOrEmpty(tagAttribute.DocumentationUrl))
            {
                tag.ExternalDocumentation = new OpenApiExternalDocumentation
                {
                    Description = tagAttribute.DocumentationDescription,
                    Url = tagAttribute.DocumentationUrl
                };
            }
        }
    }
}
