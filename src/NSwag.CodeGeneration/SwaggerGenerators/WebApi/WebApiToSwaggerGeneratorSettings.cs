//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi.Processors;

namespace NSwag.CodeGeneration.SwaggerGenerators.WebApi
{
    /// <summary>Settings for the <see cref="WebApiToSwaggerGenerator"/>.</summary>
    public class WebApiToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="WebApiToSwaggerGeneratorSettings"/> class.</summary>
        public WebApiToSwaggerGeneratorSettings()
        {
            NullHandling = NullHandling.Swagger;
        }

        /// <summary>Gets or sets the default Web API URL template.</summary>
        public string DefaultUrlTemplate { get; set; } =  "api/{controller}/{action}/{id}";

        /// <summary>Gets or sets the specification title.</summary>
        /// <value>The title.</value>
        public string Title { get; set; } = "Web API Swagger specification";

        /// <summary>Gets or sets the specification version.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>Gets the operation processor.</summary>
        [JsonIgnore]
        public IList<IOperationProcessor> OperationProcessors { get; } = new List<IOperationProcessor>();
    }
}