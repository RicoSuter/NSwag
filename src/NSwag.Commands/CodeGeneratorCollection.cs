using Newtonsoft.Json;
using NSwag.Commands.CodeGeneration;

namespace NSwag.Commands
{
    /// <summary>The command collection.</summary>
#pragma warning disable CA1711
    public class CodeGeneratorCollection
#pragma warning restore CA1711
    {
        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonProperty("OpenApiToTypeScriptClient", NullValueHandling = NullValueHandling.Ignore)]
        public OpenApiToTypeScriptClientCommand OpenApiToTypeScriptClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonProperty("OpenApiToCSharpClient", NullValueHandling = NullValueHandling.Ignore)]
        public OpenApiToCSharpClientCommand OpenApiToCSharpClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonProperty("OpenApiToCSharpController", NullValueHandling = NullValueHandling.Ignore)]
        public OpenApiToCSharpControllerCommand OpenApiToCSharpControllerCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<InputOutputCommandBase> Items => new InputOutputCommandBase[]
        {
            OpenApiToTypeScriptClientCommand,
            OpenApiToCSharpClientCommand,
            OpenApiToCSharpControllerCommand
        }.Where(cmd => cmd != null);
    }
}