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
using System.Reflection;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Processes the SwaggerTagAttribute and SwaggerTagsAttribute on the controller classes.</summary>
    public class DocumentTagsProcessor : IDocumentProcessor
    {
        /// <summary>Processes the specified Swagger document.</summary>
        /// <param name="document">The document.</param>
        /// <param name="controllerTypes">The controller types.</param>
        public void Process(SwaggerService document, IEnumerable<Type> controllerTypes)
        {
            foreach (var controllerType in controllerTypes)
            {
                ProcessSwaggerTagsAttribute(document, controllerType);
                ProcessSwaggerTagAttributes(document, controllerType);
            }
        }

        private static void ProcessSwaggerTagsAttribute(SwaggerService document, Type controllerType)
        {
            dynamic tagsAttribute = controllerType.GetTypeInfo().GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "SwaggerTagsAttribute");

            if (tagsAttribute != null)
            {
                var tags = ((string[])tagsAttribute.Tags)
                    .Select(t => new SwaggerTag { Name = t })
                    .ToList();

                if (tags.Any())
                {
                    if (document.Tags == null)
                        document.Tags = new List<SwaggerTag>();

                    foreach (var tag in tags)
                        document.Tags.Add(tag);
                }
            }
        }

        private static void ProcessSwaggerTagAttributes(SwaggerService document, Type controllerType)
        {
            var tagAttributes = controllerType.GetTypeInfo().GetCustomAttributes()
                .Where(a => a.GetType().Name == "SwaggerTagAttribute")
                .Select(a => (dynamic)a)
                .ToArray();

            if (tagAttributes.Any())
            {
                if (document.Tags == null)
                    document.Tags = new List<SwaggerTag>();

                foreach (var tagAttribute in tagAttributes)
                {
                    var tag = new SwaggerTag
                    {
                        Name = tagAttribute.Name,
                        Description = tagAttribute.Description
                    };

                    if (!string.IsNullOrEmpty(tagAttribute.DocumentationDescription) ||
                        !string.IsNullOrEmpty(tagAttribute.DocumentationUrl))
                    {
                        tag.ExternalDocumentation = new SwaggerExternalDocumentation
                        {
                            Description = tagAttribute.DocumentationDescription,
                            Url = tagAttribute.DocumentationUrl
                        };
                    }

                    document.Tags.Add(tag);
                }
            }
        }
    }
}
