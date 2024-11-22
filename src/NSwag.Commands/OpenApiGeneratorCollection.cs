﻿using Newtonsoft.Json;
using NSwag.Commands.CodeGeneration;
using NSwag.Commands.Generation;
using NSwag.Commands.Generation.AspNetCore;

namespace NSwag.Commands
{
    /// <summary></summary>
#pragma warning disable CA1711
    public class OpenApiGeneratorCollection
#pragma warning restore CA1711
    {
        /// <summary>Gets or sets the input to swagger command.</summary>
        [JsonIgnore]
        public FromDocumentCommand FromDocumentCommand { get; set; }

        /// <summary>Gets or sets the json schema to swagger command.</summary>
        [JsonIgnore]
        public JsonSchemaToOpenApiCommand JsonSchemaToOpenApiCommand { get; set; }

        /// <summary>Gets or sets the ASP.NET Core to swagger command.</summary>
        [JsonIgnore]
        public AspNetCoreToOpenApiCommand AspNetCoreToOpenApiCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<IOutputCommand> Items =>
        [
            FromDocumentCommand,
            JsonSchemaToOpenApiCommand,
            AspNetCoreToOpenApiCommand
        ];
    }
}