//-----------------------------------------------------------------------
// <copyright file="AspNetCoreOperationTagsProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Controllers;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.AspNetCore.Processors
{
    /// <summary>Processes the SwaggerTagsAttribute on the operation method.</summary>
    public class AspNetCoreOperationTagsProcessor : OperationTagsProcessor
    {
        protected override void AddControllerNameTag(OperationProcessorContext context)
        {
            var aspNetCoreContext = (AspNetCoreOperationProcessorContext)context;
            if (aspNetCoreContext.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
            {
                aspNetCoreContext.OperationDescription.Operation.Tags.Add(descriptor.ControllerName);
                return;
            }

            base.AddControllerNameTag(context);
        }
    }
}