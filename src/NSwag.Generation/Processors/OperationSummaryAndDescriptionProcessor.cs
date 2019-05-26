//-----------------------------------------------------------------------
// <copyright file="OperationSummaryAndDescriptionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
            dynamic descriptionAttribute = context.MethodInfo.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
            {
                context.OperationDescription.Operation.Summary = descriptionAttribute.Description;
            }
            else
            {
                var summary = context.MethodInfo.GetXmlDocsSummary();
                if (summary != string.Empty)
                {
                    context.OperationDescription.Operation.Summary = summary;
                }
            }

            var remarks = context.MethodInfo.GetXmlDocsRemarks();
            if (remarks != string.Empty)
            {
                context.OperationDescription.Operation.Description = remarks;
            }

            return true; 
        }
    }
}