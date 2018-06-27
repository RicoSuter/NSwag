//-----------------------------------------------------------------------
// <copyright file="ApiVersionProcessor.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.Processors
{
    /// <summary>An operation processor which replaces {version:apiVersion} route placeholders and filters the included versions.</summary>
    public class ApiVersionProcessor : IOperationProcessor
    {
        /// <summary>Gets or sets the list of versions which should be included in the generated document (leave empty to include all versions).</summary>
        public string[] IncludedVersions { get; set; }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context">The processor context.</param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            var versions = GetVersions(context, "ApiVersionAttribute");
            if (versions.Any())
            {
                if (versions.Any(v => IncludedVersions == null || IncludedVersions.Length == 0 || IncludedVersions.Contains(v)))
                {
                    var mappedVersions = GetVersions(context, "MapToApiVersionAttribute");

                    var version = mappedVersions.FirstOrDefault(v => IncludedVersions == null || IncludedVersions.Length == 0 || IncludedVersions.Contains(v));
                    if (version == null && mappedVersions.Length == 0)
                        version = IncludedVersions != null && IncludedVersions.Any() ? IncludedVersions[0] : versions[0];

                    if (version != null)
                    {
                        var operationDescription = context.OperationDescription;
                        operationDescription.Path = operationDescription.Path.Replace("{version:apiVersion}", version);
                        operationDescription.Path = operationDescription.Path.Replace("{version}", version);

                        return Task.FromResult(true);
                    }
                    else
                        return Task.FromResult(false);
                }

                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private string[] GetVersions(OperationProcessorContext context, string attributeType)
        {
            var versionAttributes = context.MethodInfo.GetCustomAttributes()
                .Concat(context.MethodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes())
                .Concat(context.ControllerType.GetTypeInfo().GetCustomAttributes(true))
                .Where(a => a.GetType().IsAssignableTo(attributeType, TypeNameStyle.Name) && a.HasProperty("Versions"))
                .SelectMany((dynamic a) => ((IEnumerable)a.Versions).OfType<object>().Select(v => v.ToString()))
                .ToArray();

            return versionAttributes;
        }
    }
}
