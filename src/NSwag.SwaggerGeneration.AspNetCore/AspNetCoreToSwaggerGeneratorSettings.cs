//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.AspNetCore.Processors;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Settings for the <see cref="AspNetCoreToSwaggerGenerator"/>.</summary>
    public class AspNetCoreToSwaggerGeneratorSettings : SwaggerGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreToSwaggerGeneratorSettings"/> class.</summary>
        public AspNetCoreToSwaggerGeneratorSettings()
        {
            OperationProcessors.Insert(2, new OperationParameterProcessor(this));
            OperationProcessors.Insert(2, new OperationResponseProcessor(this));
            OperationProcessors.Replace<OperationTagsProcessor>(new AspNetCoreOperationTagsProcessor());
        }

        /// <summary>Gets or sets the ASP.NET Core API Explorer group names to include (default: empty/null = all, often used to select API version).</summary>
        public string[] ApiGroupNames { get; set; }

        /// <summary>Gets or sets a value indicating whether parameters without default value are always required
        /// (legacy, default: false).</summary>
        /// <remarks>Use BindRequiredAttribute to mark parameters as required.</remarks>
        public bool RequireParametersWithoutDefault { get; set; }
    }
}