//-----------------------------------------------------------------------
// <copyright file="OutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;

namespace NSwag.Commands
{
    public abstract class OutputCommandBase : IOutputCommand
    {
        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
        [JsonProperty("output", NullValueHandling = NullValueHandling.Include)]
        public string OutputFilePath { get; set; }

        public abstract Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected async Task<SwaggerDocument> ReadSwaggerDocumentAsync(string input)
        {
            if (!IsJson(input) && !IsYaml(input))
            {
                if (input.StartsWith("http://") || input.StartsWith("https://"))
                {
                    if (input.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
                        return await SwaggerYamlDocument.FromUrlAsync(input).ConfigureAwait(false);
                    else
                        return await SwaggerDocument.FromUrlAsync(input).ConfigureAwait(false);
                }
                else
                {
                    if (input.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase))
                        return await SwaggerYamlDocument.FromFileAsync(input).ConfigureAwait(false);
                    else
                        return await SwaggerDocument.FromFileAsync(input).ConfigureAwait(false);
                }
            }               
            else
            {
                if (IsYaml(input))
                    return await SwaggerYamlDocument.FromYamlAsync(input).ConfigureAwait(false);
                else
                    return await SwaggerDocument.FromJsonAsync(input).ConfigureAwait(false);
            }
        }

        protected bool IsJson(string data)
        {
            return data.StartsWith("{");
        }

        protected bool IsYaml(string data)
        {
            return !IsJson(data) && data.Contains("\n");
        }

        protected Task<bool> TryWriteFileOutputAsync(IConsoleHost host, Func<string> generator)
        {
            return OutputCommandExtensions.TryWriteFileOutputAsync(this, host, generator);
        }

        protected Task<bool> TryWriteDocumentOutputAsync(IConsoleHost host, Func<SwaggerDocument> generator)
        {
            return OutputCommandExtensions.TryWriteDocumentOutputAsync(this, host, generator);
        }

        protected Task<bool> TryWriteFileOutputAsync(string path, IConsoleHost host, Func<string> generator)
        {
            return OutputCommandExtensions.TryWriteFileOutputAsync(this, path, host, generator);
        }
    }
}