//-----------------------------------------------------------------------
// <copyright file="OperationSummaryAndDescriptionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NJsonSchema;
using NJsonSchema.Infrastructure;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors
{
    /// <summary>Loads the operation summary and description from the DescriptionAttribute and the XML documentation.</summary>
    public class OperationSummaryAndDescriptionProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="document">The Swagger document.</param>
        /// <param name="operationDescription">The operation description.</param>
        /// <param name="methodInfo">The method information.</param>
        /// <param name="swaggerGenerator">The swagger generator.</param>
        /// <param name="allOperationDescriptions">All operation descriptions.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public bool Process(SwaggerDocument document, SwaggerOperationDescription operationDescription, MethodInfo methodInfo, 
            SwaggerGenerator swaggerGenerator, IList<SwaggerOperationDescription> allOperationDescriptions)
        {
            dynamic descriptionAttribute = methodInfo.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");

            if (descriptionAttribute != null)
                operationDescription.Operation.Summary = descriptionAttribute.Description;
            else
            {
                var summary = methodInfo.GetXmlSummary();
                if (summary != string.Empty)
                    operationDescription.Operation.Summary = summary;
            }

            var remarks = methodInfo.GetXmlRemarks();
            if (remarks != string.Empty)
                operationDescription.Operation.Description = remarks;

            return true; 
        }
    }
}