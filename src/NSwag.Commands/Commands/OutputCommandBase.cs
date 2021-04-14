//-----------------------------------------------------------------------
// <copyright file="OutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
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

        [Argument(Name = "NewLineBehavior", IsRequired = false, Description = "The new line behavior (Auto (OS default), CRLF, LF).")]
        [JsonProperty("newLineBehavior", NullValueHandling = NullValueHandling.Include)]
        public NewLineBehavior NewLineBehavior { get; set; } = NewLineBehavior.Auto;

        public abstract Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected Task<OpenApiDocument> ReadSwaggerDocumentAsync(string input)
        {
            if (!IsJson(input) && !IsYaml(input))
            {
                if (input.StartsWith("http://") || input.StartsWith("https://"))
                {
                    if (input.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) ||
                        input.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiYamlDocument.FromUrlAsync(input);
                    }
                    else
                    {
                        return OpenApiDocument.FromUrlAsync(input);
                    }
                }
                else
                {
                    if (input.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) ||
                        input.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
                    {
                        return OpenApiYamlDocument.FromFileAsync(input);
                    }
                    else
                    {
                        return OpenApiDocument.FromFileAsync(input);
                    }
                }
            }
            else
            {
                if (IsYaml(input))
                {
                    return OpenApiYamlDocument.FromYamlAsync(input);
                }
                else
                {
                    return OpenApiDocument.FromJsonAsync(input);
                }
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
            return OutputCommandExtensions.TryWriteFileOutputAsync(this, host, NewLineBehavior, generator);
        }

        protected Task<bool> TryWriteDocumentOutputAsync(IConsoleHost host, Func<OpenApiDocument> generator)
        {
            return OutputCommandExtensions.TryWriteDocumentOutputAsync(this, host, NewLineBehavior, generator);
        }

        protected Task<bool> TryWriteFileOutputAsync(string path, IConsoleHost host, Func<string> generator)
        {
            return OutputCommandExtensions.TryWriteFileOutputAsync(this, path, host, NewLineBehavior, generator);
        }
    }
}