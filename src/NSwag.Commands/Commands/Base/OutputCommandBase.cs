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
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands.Base
{
    public abstract class OutputCommandBase : IConsoleCommand
    {
        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
        [JsonProperty("output", NullValueHandling = NullValueHandling.Include)]
        public string OutputFilePath { get; set; }

        public abstract Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host);

        protected Task<bool> TryWriteFileOutputAsync(IConsoleHost host, Func<string> generator)
        {
            return TryWriteFileOutputAsync(OutputFilePath, host, generator);
        }

        protected async Task<bool> TryWriteFileOutputAsync(string path, IConsoleHost host, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var directory = DynamicApis.PathGetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && await DynamicApis.DirectoryExistsAsync(directory).ConfigureAwait(false) == false)
                    await DynamicApis.DirectoryCreateDirectoryAsync(directory).ConfigureAwait(false);

                var data = generator();
                await DynamicApis.FileWriteAllTextAsync(path, data).ConfigureAwait(false);
                host?.WriteMessage("Code has been successfully written to file.\n");

                return true; 
            }
            return false;
        }
    }
}