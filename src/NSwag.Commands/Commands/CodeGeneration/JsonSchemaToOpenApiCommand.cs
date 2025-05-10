﻿using System.Text.RegularExpressions;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;

#pragma warning disable 1591

namespace NSwag.Commands.CodeGeneration
{
    public class JsonSchemaToOpenApiCommand : OutputCommandBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("schema")]
        public string Schema { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await RunAsync();
        }

        public async Task<OpenApiDocument> RunAsync()
        {
            var schema = await JsonSchema.FromJsonAsync(Schema).ConfigureAwait(false);
            var document = new OpenApiDocument();

            var rootSchemaName = string.IsNullOrEmpty(Name) && Regex.IsMatch(schema.Title ?? string.Empty, "^[a-zA-Z0-9_]*$") ? schema.Title : Name;
            if (string.IsNullOrEmpty(rootSchemaName))
            {
                rootSchemaName = "Anonymous";
            }

            document.Definitions[rootSchemaName] = schema;
            return document;
        }
    }
}
