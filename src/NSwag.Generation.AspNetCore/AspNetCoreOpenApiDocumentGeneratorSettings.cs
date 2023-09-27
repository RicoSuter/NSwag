//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Generation.Processors;
using NSwag.Generation.AspNetCore.Processors;
using System;

namespace NSwag.Generation.AspNetCore
{
    /// <summary>Settings for the <see cref="AspNetCoreOpenApiDocumentGenerator"/>.</summary>
    public class AspNetCoreOpenApiDocumentGeneratorSettings : OpenApiDocumentGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreOpenApiDocumentGeneratorSettings"/> class.</summary>
        public AspNetCoreOpenApiDocumentGeneratorSettings()
        {
            OperationProcessors.Insert(2, new OperationParameterProcessor(this));
            OperationProcessors.Insert(2, new OperationResponseProcessor(this));
            OperationProcessors.Replace<OperationTagsProcessor>(new AspNetCoreOperationTagsProcessor());
        }

        /// <summary>Gets the document name (internal identifier, default: v1).</summary>
        public string DocumentName { get; set; } = "v1";

        /// <summary>Gets or sets the ASP.NET Core API Explorer group names to include (default: empty/null = all, often used to select API version).</summary>
        public string[] ApiGroupNames { get; set; }

        /// <summary>Gets or sets a value indicating whether parameters without default value are always required
        /// (legacy, default: false).</summary>
        /// <remarks>Use BindRequiredAttribute to mark parameters as required.</remarks>
        public bool RequireParametersWithoutDefault { get; set; }

        /// <summary>Gets or sets the Swagger post process action.</summary>
        public Action<OpenApiDocument> PostProcess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a route name associated with an action is used to generate its operationId.
        /// </summary>
        /// <remarks>If OpenApiOperationAttribute is present, it will be preferred over the route name irrespective of this property.</remarks>
        public bool UseRouteNameAsOperationId { get; set; }
    }
}