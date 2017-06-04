//-----------------------------------------------------------------------
// <copyright file="ApiVersionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;
using NSwag.SwaggerGeneration.WebApi.Infrastructure;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    public class ApiVersionProcessor : IOperationProcessor
    {
        public Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            var versionAttributes = context.MethodInfo.GetCustomAttributes()
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes())
                .Where(a => a.GetType().IsAssignableTo("MapToApiVersionAttribute", TypeNameStyle.Name) ||
                            a.GetType().IsAssignableTo("ApiVersionAttribute", TypeNameStyle.Name))
                .Select(a => (dynamic)a)
                .ToArray();

            if (versionAttributes.Any())
            {
                var versionAttribute = versionAttributes.First();
                if (ObjectExtensions.HasProperty(versionAttribute, "Versions"))
                {
                    ReplaceApiVersionInPath(context.OperationDescription, versionAttribute.Versions);
                }
            }

            return Task.FromResult(true);
        }

        private void ReplaceApiVersionInPath(SwaggerOperationDescription operationDescription, dynamic versions)
        {
            operationDescription.Path = operationDescription.Path.Replace("{version:apiVersion}", versions[0].ToString());
        }
    }
}
