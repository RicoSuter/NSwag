//-----------------------------------------------------------------------
// <copyright file="DocumentTagsProcessor.cs" company="NSwag">
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
    /// <summary>Processes the SwaggerExtensionDataAttribute on the controller classes.</summary>
    public class DocumentExtensionDataProcessor : IDocumentProcessor
    {
        /// <summary>
        /// Processes the specified Swagger document.
        /// </summary>
        /// <param name="context">The processor context.</param>
        public void Process(DocumentProcessorContext context)
        {
            if (context.Document.ExtensionData == null)
            {
                context.Document.ExtensionData = new Dictionary<string, object>();
            }

            foreach (var extensionDataAttribute in
                from type in context.AllControllerTypes
                from extensionDataAttribute in type.GetTypeInfo().GetCustomAttributes(true)
                    .GetAssignableToTypeName("SwaggerExtensionDataAttribute", TypeNameStyle.Name)
                select (dynamic)extensionDataAttribute)
            {
                string key = extensionDataAttribute.Key;
                string value = extensionDataAttribute.Value;

                context.Document.ExtensionData[key] = value;
            }
        }
    }
}
