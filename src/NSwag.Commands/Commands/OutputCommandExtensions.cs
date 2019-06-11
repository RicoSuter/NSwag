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

namespace NSwag.Commands
{
    public static class OutputCommandExtensions
    {
        public static Task<bool> TryWriteFileOutputAsync(this IOutputCommand command, IConsoleHost host, Func<string> generator)
        {
            return TryWriteFileOutputAsync(command, command.OutputFilePath, host, generator);
        }

        public static Task<bool> TryWriteDocumentOutputAsync(this IOutputCommand command, IConsoleHost host, Func<OpenApiDocument> generator)
        {
            return TryWriteFileOutputAsync(command, command.OutputFilePath, host, () =>
                command.OutputFilePath.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) ? OpenApiYamlDocument.ToYaml(generator()) : generator().ToJson());
        }

        public static async Task<bool> TryWriteFileOutputAsync(this IOutputCommand command, string path, IConsoleHost host, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var directory = DynamicApis.PathGetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && DynamicApis.DirectoryExists(directory) == false)
                {
                    DynamicApis.DirectoryCreateDirectory(directory);
                }

                var data = generator();
                if (!DynamicApis.FileExists(path) || DynamicApis.FileReadAllText(path) != data)
                {
                    DynamicApis.FileWriteAllText(path, data);
                    host?.WriteMessage("Code has been successfully written to file.\n");
                }
                else
                {
                    host?.WriteMessage("Code has been successfully generated but not written to file (no change detected).\n");
                }
                return true; 
            }
            return false;
        }
    }
}