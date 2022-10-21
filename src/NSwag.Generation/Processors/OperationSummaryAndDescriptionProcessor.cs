//-----------------------------------------------------------------------
// <copyright file="OperationSummaryAndDescriptionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Namotion.Reflection;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation.Processors
{
    /// <summary>Loads the operation summary and description from the DescriptionAttribute and the XML documentation.</summary>
    public class OperationSummaryAndDescriptionProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The operation processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            var attributes = context.MethodInfo?.GetCustomAttributes().ToArray() ?? new Attribute[0];

            ProcessSummary(context, attributes);
            ProcessDescription(context, attributes);

            return true;
        }

        private void ProcessSummary(OperationProcessorContext context, Attribute[] attributes)
        {
            dynamic openApiOperationAttribute = attributes
                .SingleOrDefault(a => a.GetType().Name == "OpenApiOperationAttribute");

            string summary = openApiOperationAttribute?.Summary;

            if (string.IsNullOrEmpty(summary))
            {
                dynamic descriptionAttribute = attributes
                    .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

                summary = descriptionAttribute?.Description;
            }

            if (string.IsNullOrEmpty(summary))
            {
                summary = context.MethodInfo?.GetXmlDocsSummary(context.Settings.GetXmlDocsOptions());
            }

            if (!string.IsNullOrEmpty(summary))
            {
                context.OperationDescription.Operation.Summary = summary.Trim();
            }
        }

        private void ProcessDescription(OperationProcessorContext context, Attribute[] attributes)
        {
            dynamic openApiOperationAttribute = attributes
                .SingleOrDefault(a => a.GetType().Name == "OpenApiOperationAttribute");

            string description = openApiOperationAttribute?.Description;

            if (string.IsNullOrEmpty(description))
            {
                description = context.MethodInfo?.GetXmlDocsRemarks(context.Settings.GetXmlDocsOptions());
            }

            if (!string.IsNullOrEmpty(description))
            {
                context.OperationDescription.Operation.Description = description.Trim();
            }
        }
    }
}