//-----------------------------------------------------------------------
// <copyright file="OutputCommandBase.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Threading.Tasks;
using NConsole;
using NJsonSchema.Infrastructure;

#pragma warning disable 1591

namespace NSwag.Commands
{
    public static class OutputCommandExtensions
    {
        public static Task<bool> TryWriteFileOutputAsync(this IOutputCommand command, IConsoleHost host, NewLineBehavior newLineBehavior, Func<string> generator)
        {
            return TryWriteFileOutputAsync(command, command.OutputFilePath, host, newLineBehavior, generator);
        }

        public static Task<bool> TryWriteDocumentOutputAsync(this IOutputCommand command, IConsoleHost host, NewLineBehavior newLineBehavior, Func<OpenApiDocument> generator)
        {
            return TryWriteFileOutputAsync(command, command.OutputFilePath, host, newLineBehavior, () =>
                command.OutputFilePath.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase) ? OpenApiYamlDocument.ToYaml(generator()) : generator().ToJson());
        }

        public static Task<bool> TryWriteFileOutputAsync(this IOutputCommand command, string path, IConsoleHost host, NewLineBehavior newLineBehavior, Func<string> generator)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory) == false)
                {
                    Directory.CreateDirectory(directory);
                }

                var data = generator();

                data = data?.Replace("\r", "") ?? "";
                data = newLineBehavior == NewLineBehavior.Auto ? data.Replace("\n", Environment.NewLine) :
                       newLineBehavior == NewLineBehavior.CRLF ? data.Replace("\n", "\r\n") : data;

                if (!File.Exists(path) || File.ReadAllText(path) != data)
                {
                    File.WriteAllText(path, data);

                    host?.WriteMessage("Code has been successfully written to file.\n");
                }
                else
                {
                    host?.WriteMessage("Code has been successfully generated but not written to file (no change detected).\n");
                }
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}