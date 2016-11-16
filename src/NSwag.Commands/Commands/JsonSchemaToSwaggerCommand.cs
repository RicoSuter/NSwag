using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    /// <summary></summary>
    public class JsonSchemaToSwaggerCommand : OutputCommandBase
    {
        /// <summary>Gets or sets the input JSON Schema.</summary>
        [JsonProperty("Schema")]
        public string Schema { get; set; }

        /// <summary>Runs the asynchronous.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return await RunAsync();
        }

        /// <summary></summary>
        public async Task<SwaggerDocument> RunAsync()
        {
            var schema = await Task.Run(() => JsonSchema4.FromJson(Schema));
            var document = new SwaggerDocument();
            document.Definitions[schema.TypeNameRaw ?? "MyType"] = schema;
            return document;
        }
    }
}
