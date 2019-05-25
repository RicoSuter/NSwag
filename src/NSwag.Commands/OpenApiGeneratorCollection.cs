using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag.Commands.CodeGeneration;
using NSwag.Commands.SwaggerGeneration;
using NSwag.Commands.SwaggerGeneration.AspNetCore;
using NSwag.Commands.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    /// <summary></summary>
    public class OpenApiGeneratorCollection
    {
        /// <summary>Gets or sets the input to swagger command.</summary>
        [JsonIgnore]
        public FromDocumentCommand FromSwaggerCommand { get; set; }

        /// <summary>Gets or sets the json schema to swagger command.</summary>
        [JsonIgnore]
        public JsonSchemaToOpenApiCommand JsonSchemaToOpenApiCommand { get; set; }

        /// <summary>Gets or sets the Web API to swagger command.</summary>
        [JsonIgnore]
        public WebApiToOpenApiCommand WebApiToOpenApiCommand { get; set; }

        /// <summary>Gets or sets the ASP.NET Core to swagger command.</summary>
        [JsonIgnore]
        public AspNetCoreToOpenApiCommand AspNetCoreToOpenApiCommand { get; set; }

        /// <summary>Gets or sets the assembly type to swagger command.</summary>
        [JsonIgnore]
        public TypesToOpenApiCommand TypesToOpenApiCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<IOutputCommand> Items => new IOutputCommand[]
        {
            FromSwaggerCommand,
            JsonSchemaToOpenApiCommand,
            WebApiToOpenApiCommand,
            TypesToOpenApiCommand,
            AspNetCoreToOpenApiCommand
        };
    }
}