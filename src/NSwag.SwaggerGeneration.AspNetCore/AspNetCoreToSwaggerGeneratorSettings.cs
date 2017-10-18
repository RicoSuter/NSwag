//-----------------------------------------------------------------------
// <copyright file="WebApiToSwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema.Generation;
using NSwag.SwaggerGeneration.WebApi.Processors;

namespace NSwag.SwaggerGeneration.AspNetCore
{
    /// <summary>Settings for the <see cref="AspNetCoreToSwaggerGeneratorSettings"/>.</summary>
    public class AspNetCoreToSwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="AspNetCoreToSwaggerGeneratorSettings"/> class.</summary>
        public AspNetCoreToSwaggerGeneratorSettings()
        {
            SchemaType = NJsonSchema.SchemaType.Swagger2;
            OperationProcessors.Add(new OperationParameterProcessor(this));
            OperationProcessors.Add(new OperationResponseProcessor(this));
        }

        /// <summary>Gets or sets the Swagger specification title.</summary>
        public string Title { get; set; } = "Web API Swagger specification";

        /// <summary>Gets or sets the Swagger specification description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the Swagger specification version.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>Gets the operation processor.</summary>
        [JsonIgnore]
        public OperationProcessorCollection OperationProcessors { get; } = new OperationProcessorCollection
        {
            new ApiVersionProcessor(),
            new OperationSummaryAndDescriptionProcessor(),
            new OperationTagsProcessor()
        };

        /// <summary>Gets the operation processor.</summary>
        [JsonIgnore]
        public DocumentProcessorCollection DocumentProcessors { get; } = new DocumentProcessorCollection
        {
            new DocumentTagsProcessor()
        };

        /// <summary>Gets or sets the document template representing the initial Swagger specification (JSON data).</summary>
        public string DocumentTemplate { get; set; }
    }
}