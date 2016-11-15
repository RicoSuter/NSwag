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

        protected bool TryWriteFileOutput(IConsoleHost host, Func<string> generator)
        {
            return TryWriteFileOutput(OutputFilePath, host, generator);
        }

        protected bool TryWriteFileOutput(string path, IConsoleHost host, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var directory = DynamicApis.PathGetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !DynamicApis.DirectoryExists(directory))
                    DynamicApis.DirectoryCreateDirectory(directory);
                
                DynamicApis.FileWriteAllText(path, generator());
                host?.WriteMessage("Code has been successfully written to file.\n");

                return true; 
            }
            return false;
        }
    }
}