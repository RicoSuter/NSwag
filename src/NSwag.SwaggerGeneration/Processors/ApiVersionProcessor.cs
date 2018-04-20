//-----------------------------------------------------------------------
// <copyright file="ApiVersionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.Processors
{
    /// <summary>An operation processor which replaces {version:apiVersion} route placeholders and filters the included versions.</summary>
    public class ApiVersionProcessor : IOperationProcessor
    {
        /// <summary>Gets the list of versions which should be included in the generated document (leave empty to include all versions).</summary>
        public IList<string> IncludedVersions { get; } = new List<string>();

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            var versionAttributes = context.MethodInfo.GetCustomAttributes()
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes())
                .Concat(context.ControllerType.GetTypeInfo().GetCustomAttributes())
                .Where(a => a.GetType().IsAssignableTo("MapToApiVersionAttribute", TypeNameStyle.Name) ||
                            a.GetType().IsAssignableTo("ApiVersionAttribute", TypeNameStyle.Name))
                .Select(a => (dynamic)a)
                .ToArray();

            var versionAttribute = versionAttributes.FirstOrDefault();
            if (ReflectionExtensions.HasProperty(versionAttribute, "Versions"))
            {
                var versions = ((IEnumerable)versionAttribute.Versions)
                    .OfType<object>()
                    .Select(v => v.ToString())
                    .ToArray();

                var version = versions.FirstOrDefault(v => IncludedVersions.Count == 0 || IncludedVersions.Contains(v));
                if (version != null)
                {
                    var operationDescription = context.OperationDescription;
                    operationDescription.Path = operationDescription.Path.Replace("{version:apiVersion}", version);

                    return Task.FromResult(true);
                }
                else
                    return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }
}
