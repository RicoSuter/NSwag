//-----------------------------------------------------------------------
// <copyright file="SwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Net;
using Namotion.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Collections;
using NSwag.Generation.Processors.Contexts;

namespace NSwag.Generation
{
    /// <summary>Settings for the Swagger generator.</summary>
    public class OpenApiDocumentGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiDocumentGeneratorSettings"/> class.</summary>
        public OpenApiDocumentGeneratorSettings()
        {
            SchemaGeneratorFactory = () => new OpenApiSchemaGenerator(this);
            DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
        }

        /// <summary>Gets or sets the JSON Schema generator factory (default: new instance of <see cref="OpenApiSchemaGenerator"/>.</summary>
        public Func<OpenApiSchemaGenerator> SchemaGeneratorFactory { get; set; }

        /// <summary></summary>
        public JsonSchemaGeneratorSettings SchemaSettings { get; set; } = new SystemTextJsonSchemaGeneratorSettings
        {
            SchemaType = SchemaType.OpenApi3
        };

        /// <summary>Gets or sets the Swagger specification title.</summary>
        public string Title { get; set; } = "My Title";

        /// <summary>Gets or sets the Swagger specification description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the Swagger specification version.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>Gets or sets a value indicating whether nullable body parameters are allowed (ignored when MvcOptions.AllowEmptyInputInBodyModelBinding is available (ASP.NET Core 2.0+), default: true).</summary>
        public bool AllowNullableBodyParameters { get; set; } = true;

        /// <summary>Gets or sets the default response reference type null handling when no nullability information is available (if NotNullAttribute and CanBeNullAttribute are missing, default: NotNull).</summary>
        public ReferenceTypeNullHandling DefaultResponseReferenceTypeNullHandling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating that the api method/action response should be considered nullable if this response type is documented, even if it is a void response.
        /// <para>Allows for things like a 204 No Content to be treated as nullable without decorating with the <see cref="NJsonSchema.Annotations.CanBeNullAttribute"/></para>
        /// <para>If the action is decorated with the <see cref="NJsonSchema.Annotations.NotNullAttribute"/></para> this setting will be ignored for that action.
        /// </summary>
        public HttpStatusCode[] ResponseStatusCodesToTreatAsNullable { get; set; } = [];

        /// <summary>Gets or sets a value indicating whether to generate x-originalName properties when parameter name is different in .NET and HTTP (default: true).</summary>
        public bool GenerateOriginalParameterNames { get; set; } = true;

        /// <summary>Gets the operation processors.</summary>
        [JsonIgnore]
        public OperationProcessorCollection OperationProcessors { get; } =
        [
            new OperationSummaryAndDescriptionProcessor(),
            new OperationTagsProcessor(),
            new OperationExtensionDataProcessor()
        ];

        /// <summary>Gets the document processors.</summary>
        [JsonIgnore]
        public DocumentProcessorCollection DocumentProcessors { get; } =
        [
            new DocumentTagsProcessor(),
            new DocumentExtensionDataProcessor()
        ];

        /// <summary>Gets or sets the document template representing the initial Swagger specification (JSON data).</summary>
        public string DocumentTemplate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether controllers' XML documentation will be used as tag descriptions (but only when the controller name is used as a tag, default: false).
        /// </summary>
        public bool UseControllerSummaryAsTagDescription { get; set; }

        /// <summary>Gets or sets a value indicating whether the HttpMethodAttribute Name property shall be used as OperationId.</summary>
        public bool UseHttpAttributeNameAsOperationId { get; set; }

        /// <summary>Inserts a function based operation processor at the beginning of the pipeline to be used to filter operations.</summary>
        /// <param name="filter">The processor filter.</param>
        public void AddOperationFilter(Func<OperationProcessorContext, bool> filter)
        {
            OperationProcessors.Insert(0, new OperationProcessor(filter));
        }

        /// <summary>Applies the given settings to this settings object.</summary>
        /// <param name="schemaSettings">The schema generator settings.</param>
        /// <param name="mvcOptions">The MVC options.</param>
        public void ApplySettings(JsonSchemaGeneratorSettings schemaSettings, object mvcOptions)
        {
            if (schemaSettings != null)
            {
                SchemaSettings = schemaSettings;
            }

            if (mvcOptions != null && mvcOptions.HasProperty("AllowEmptyInputInBodyModelBinding"))
            {
                AllowNullableBodyParameters = mvcOptions.TryGetPropertyValue("AllowEmptyInputInBodyModelBinding", false);
            }
        }
    }
}
