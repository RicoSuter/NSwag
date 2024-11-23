﻿//-----------------------------------------------------------------------
// <copyright file="Exe.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using NConsole;

namespace NSwag.Commands.Generation.AspNetCore
{
    internal static class Exe
    {
        public static async Task<int> RunAsync(
            string executable,
            IReadOnlyList<string> args,
            IConsoleHost console = null,
            TimeSpan? timeout = null)
        {
            var arguments = ToArguments(args);

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = console != null,
                RedirectStandardError = console != null,
            };

            console?.WriteMessage($"Executing {executable} {arguments}{Environment.NewLine}");
            using var process = Process.Start(startInfo);
            var tcs = new TaskCompletionSource<bool>();
            process.EnableRaisingEvents = true;
            process.Exited += (_, eventArgs) =>
            {
                if (process.ExitCode == 0)
                {
                    tcs.TrySetResult(true);
                }
                else
                {
#pragma warning disable CA2201
                    tcs.TrySetException(new Exception($"Process failed with non-zero exit code '{process.ExitCode}'."));
#pragma warning restore CA2201
                }
            };

            if (console != null)
            {
                process.OutputDataReceived += (_, eventArgs) => console.WriteMessage(eventArgs.Data + Environment.NewLine);
                process.ErrorDataReceived += (_, eventArgs) => console.WriteError(eventArgs.Data + Environment.NewLine);

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
            }

            var result = await Task.WhenAny(tcs.Task, Task.Delay(timeout ?? TimeSpan.FromSeconds(60 * 5))).ConfigureAwait(false);
            if (result != tcs.Task)
            {
                throw new InvalidOperationException($"Process {startInfo.FileName} timed out.");
            }
            else
            {
                console?.WriteMessage($"Done executing command. Exit Code: {process.ExitCode}.{Environment.NewLine}");
                return process.ExitCode;
            }
        }

        private static string ToArguments(IReadOnlyList<string> args)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < args.Count; i++)
            {
                var argument = args[i];
                if (i != 0)
                {
                    builder.Append(' ');
                }

                if (!argument.Contains(' '))
                {
                    builder.Append(args[i]);

                    continue;
                }

                builder.Append('"');

                var pendingBackslashs = 0;
                for (var j = 0; j < argument.Length; j++)
                {
                    switch (argument[j])
                    {
                        case '\"':
                            if (pendingBackslashs != 0)
                            {
                                builder.Append('\\', pendingBackslashs * 2);
                                pendingBackslashs = 0;
                            }
                            builder.Append("\\\"");
                            break;

                        case '\\':
                            pendingBackslashs++;
                            break;

                        default:
                            if (pendingBackslashs != 0)
                            {
                                if (pendingBackslashs == 1)
                                {
                                    builder.Append('\\');
                                }
                                else
                                {
                                    builder.Append('\\', pendingBackslashs * 2);
                                }

                                pendingBackslashs = 0;
                            }

                            builder.Append(args[i][j]);
                            break;
                    }
                }

                if (pendingBackslashs != 0)
                {
                    builder.Append('\\', pendingBackslashs * 2);
                }

                builder.Append('"');
            }

            return builder.ToString();
        }
    }
}
