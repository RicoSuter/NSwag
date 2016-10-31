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
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands.Base
{
    public abstract class OutputCommandBase : IConsoleCommand
    {
        [Argument(Name = "Output", IsRequired = false, Description = "The output file path (optional).")]
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
                // TODO: Implement this
                //var file = new FileInfo(path);
                //var directory = file.Directory;

                //if (!directory.Exists)
                //    directory.Create();

                DynamicApis.FileWriteAllText(path, generator());
                host?.WriteMessage("Code has been successfully written to file.\n");

                return true; 
            }
            return false;
        }
    }
}