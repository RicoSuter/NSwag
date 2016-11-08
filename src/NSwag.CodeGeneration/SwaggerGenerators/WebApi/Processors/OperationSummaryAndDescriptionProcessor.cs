//-----------------------------------------------------------------------
// <copyright file="OperationSummaryAndDescriptionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using NJsonSchema.Infrastructure;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors.Contexts;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Loads the operation summary and description from the DescriptionAttribute and the XML documentation.</summary>
    public class OperationSummaryAndDescriptionProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(OperationProcessorContext context)
        {
            dynamic descriptionAttribute = context.MethodInfo.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
                context.OperationDescription.Operation.Summary = descriptionAttribute.Description;
            else
            {
                var summary = context.MethodInfo.GetXmlSummary();
                if (summary != string.Empty)
                    context.OperationDescription.Operation.Summary = summary;
            }

            var remarks = context.MethodInfo.GetXmlRemarks();
            if (remarks != string.Empty)
                context.OperationDescription.Operation.Description = remarks;

            return true; 
        }
    }
}