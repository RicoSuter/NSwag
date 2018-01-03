using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NSwag.Commands.CodeGeneration;

namespace NSwag.Commands
{
    /// <summary>The command collection.</summary>
    public class CodeGeneratorCollection
    {
        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonProperty("SwaggerToTypeScriptClient", NullValueHandling = NullValueHandling.Ignore)]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonProperty("SwaggerToCSharpClient", NullValueHandling = NullValueHandling.Ignore)]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonProperty("SwaggerToCSharpController", NullValueHandling = NullValueHandling.Ignore)]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<InputOutputCommandBase> Items => new InputOutputCommandBase[]
        {
            SwaggerToTypeScriptClientCommand,
            SwaggerToCSharpClientCommand,
            SwaggerToCSharpControllerCommand
        }.Where(cmd => cmd != null);
    }
}