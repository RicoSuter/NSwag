using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Commands.Base;

#pragma warning disable 1591

namespace NSwag.Commands
{
    public class JsonSchemaToSwaggerCommand : OutputCommandBase
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("schema")]
        public string Schema { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await RunAsync();
        }

        public async Task<SwaggerDocument> RunAsync()
        {
            var schema = await Task.Run(() => JsonSchema4.FromJson(Schema));
            var document = new SwaggerDocument();
            var rootSchemaName = string.IsNullOrEmpty(Name) && Regex.IsMatch(schema.Title, "^[a-zA-Z0-9_]*$") ? schema.Title : Name;
            document.Definitions[rootSchemaName] = schema;
            return document;
        }
    }
}
