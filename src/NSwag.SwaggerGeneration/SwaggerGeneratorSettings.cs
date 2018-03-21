//-----------------------------------------------------------------------
// <copyright file="SwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Newtonsoft.Json;
using NJsonSchema.Generation;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Collections;

namespace NSwag.SwaggerGeneration
{
    /// <summary>Settings for the Swagger generator.</summary>
    public class SwaggerGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerGeneratorSettings"/> class.</summary>
        public SwaggerGeneratorSettings()
        {
            SchemaType = NJsonSchema.SchemaType.Swagger2;
        }

        /// <summary>Gets or sets the Swagger specification title.</summary>
        public string Title { get; set; } = "My Title";

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
