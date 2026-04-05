using System.Text.Json.Serialization;
using NSwag.Commands.CodeGeneration;

namespace NSwag.Commands
{
    /// <summary>The command collection.</summary>
#pragma warning disable CA1711
    public class CodeGeneratorCollection
#pragma warning restore CA1711
    {
        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonPropertyName("OpenApiToTypeScriptClient")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OpenApiToTypeScriptClientCommand OpenApiToTypeScriptClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonPropertyName("OpenApiToCSharpClient")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OpenApiToCSharpClientCommand OpenApiToCSharpClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonPropertyName("OpenApiToCSharpController")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
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