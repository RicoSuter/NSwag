//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.WebApi.Processors;

namespace NSwag.SwaggerGeneration.WebApi
{
    /// <summary>Settings for the <see cref="WebApiToSwaggerGenerator"/>.</summary>
    public class WebApiToSwaggerGeneratorSettings : SwaggerGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGeneratorSettings"/> class.</summary>
        public WebApiToSwaggerGeneratorSettings()
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