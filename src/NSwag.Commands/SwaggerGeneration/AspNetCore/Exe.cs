// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    internal static class Exe
    {
        public static int Run(
            string executable,
            IReadOnlyList<string> args,
            string workingDirectory = null,
            bool readOutput = false)
        {
            var arguments = ToArguments(args);

            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = readOutput,
                RedirectStandardError = readOutput,
            };

            if (workingDirectory != null)
            {
                startInfo.WorkingDirectory = workingDirectory;
            }

            using (var process = Process.Start(startInfo))
            {

                if (readOutput)
                {
                    string line;
                    while ((line = process.StandardOutput.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                process.WaitForExit();
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
                    builder.Append(" ");
                }

                if (argument.IndexOf(' ') == -1)
                {
                    builder.Append(args[i]);

                    continue;
                }

                builder.Append("\"");

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
                                    builder.Append("\\");
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

                builder.Append("\"");
            }

            return builder.ToString();
        }
    }
}