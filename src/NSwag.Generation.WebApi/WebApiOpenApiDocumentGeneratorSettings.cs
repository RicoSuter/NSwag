//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.Generation.Processors;
using NSwag.Generation.WebApi.Processors;

namespace NSwag.Generation.WebApi
{
    /// <summary>Settings for the <see cref="WebApiOpenApiDocumentGenerator"/>.</summary>
    public class WebApiOpenApiDocumentGeneratorSettings : OpenApiDocumentGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiOpenApiDocumentGeneratorSettings"/> class.</summary>
        public WebApiOpenApiDocumentGeneratorSettings()
        {
            OperationProcessors.Insert(0, new ApiVersionProcessor());
            OperationProcessors.Insert(3, new OperationConsumesProcessor());
            OperationProcessors.Insert(3, new OperationParameterProcessor(this));
            OperationProcessors.Insert(3, new OperationResponseProcessor(this));
        }

        /// <summary>Gets or sets the default Web API URL template (default for Web API: 'api/{controller}/{id}'; for MVC projects: '{controller}/{action}/{id?}').</summary>
        public string DefaultUrlTemplate { get; set; } = "api/{controller}/{id?}";

        /// <summary>Gets or sets a value indicating whether the controllers are hosted by ASP.NET Core.</summary>
        public bool IsAspNetCore { get; set; }

        /// <summary>Gets or sets a value indicating whether to add path parameters which are missing in the action method.</summary>
        public bool AddMissingPathParameters { get; set; }
    }
}