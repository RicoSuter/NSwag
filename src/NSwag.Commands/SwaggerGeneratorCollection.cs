using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag.Commands.CodeGeneration;
using NSwag.Commands.SwaggerGeneration;
using NSwag.Commands.SwaggerGeneration.AspNetCore;
using NSwag.Commands.SwaggerGeneration.WebApi;

namespace NSwag.Commands
{
    /// <summary></summary>
    public class SwaggerGeneratorCollection
    {
        /// <summary>Gets or sets the input to swagger command.</summary>
        [JsonIgnore]
        public FromSwaggerCommand FromSwaggerCommand { get; set; }

        /// <summary>Gets or sets the json schema to swagger command.</summary>
        [JsonIgnore]
        public JsonSchemaToSwaggerCommand JsonSchemaToSwaggerCommand { get; set; }

        /// <summary>Gets or sets the Web API to swagger command.</summary>
        [JsonIgnore]
        public WebApiToSwaggerCommand WebApiToSwaggerCommand { get; set; }

        /// <summary>Gets or sets the ASP.NET Core to swagger command.</summary>
        [JsonIgnore]
        public AspNetCoreToSwaggerCommand AspNetCoreToSwaggerCommand { get; set; }

        /// <summary>Gets or sets the assembly type to swagger command.</summary>
        [JsonIgnore]
        public TypesToSwaggerCommand TypesToSwaggerCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<IOutputCommand> Items => new IOutputCommand[]
        {
            FromSwaggerCommand,
            JsonSchemaToSwaggerCommand, 
            WebApiToSwaggerCommand, 
            TypesToSwaggerCommand,
            AspNetCoreToSwaggerCommand
        };
    }
}