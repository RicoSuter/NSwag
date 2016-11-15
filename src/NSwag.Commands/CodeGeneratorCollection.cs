using System.Collections.Generic;
using Newtonsoft.Json;
using NSwag.Commands.Base;

namespace NSwag.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CodeGeneratorCollection
    {
        /// <summary>Gets or sets the SwaggerToTypeScriptClientCommand.</summary>
        [JsonProperty("SwaggerToTypeScriptClient")]
        public SwaggerToTypeScriptClientCommand SwaggerToTypeScriptClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpClientCommand.</summary>
        [JsonProperty("SwaggerToCSharpClient")]
        public SwaggerToCSharpClientCommand SwaggerToCSharpClientCommand { get; set; }

        /// <summary>Gets or sets the SwaggerToCSharpControllerCommand.</summary>
        [JsonProperty("SwaggerToCSharpController")]
        public SwaggerToCSharpControllerCommand SwaggerToCSharpControllerCommand { get; set; }

        /// <summary>Gets the items.</summary>
        [JsonIgnore]
        public IEnumerable<InputOutputCommandBase> Items => new InputOutputCommandBase[]
        {
            SwaggerToTypeScriptClientCommand,
            SwaggerToCSharpClientCommand,
            SwaggerToCSharpControllerCommand
        };
    }
}