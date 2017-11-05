//-----------------------------------------------------------------------
// <copyright file="AssemblyOutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;

namespace NSwag.Commands
{
    /// <summary>A command which is run in isolation.</summary>
    public abstract class IsolatedSwaggerOutputCommandBase : IsolatedCommandBase<string>, IOutputCommand
    {
        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
        [JsonProperty("output", NullValueHandling = NullValueHandling.Include)]
        public string OutputFilePath { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            var documentJson = await RunIsolatedAsync(null);
            var document = await SwaggerDocument.FromJsonAsync(documentJson).ConfigureAwait(false);
            await this.TryWriteDocumentOutputAsync(host, () => document).ConfigureAwait(false);
            return document;
        }
    }
}