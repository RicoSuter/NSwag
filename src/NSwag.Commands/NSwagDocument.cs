//-----------------------------------------------------------------------
// <copyright file="NSwagDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#pragma warning disable IDE0005

using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using NSwag.Commands.Generation.AspNetCore;

namespace NSwag.Commands
{
    /// <summary>The NSwagDocument implementation.</summary>
    /// <seealso cref="NSwagDocumentBase" />
    public class NSwagDocument : NSwagDocumentBase
    {
#if NET462

        /// <summary>Gets or sets the root binary directory where the command line executables loaded from.</summary>
        public static string RootBinaryDirectory { get; set; } =
            System.IO.Path.GetDirectoryName(typeof(NSwagDocument).GetTypeInfo().Assembly.Location);

#endif
        /// <summary>Initializes a new instance of the <see cref="NSwagDocument"/> class.</summary>
        public NSwagDocument()
        {
            SwaggerGenerators.AspNetCoreToOpenApiCommand = new AspNetCoreToOpenApiCommand();
        }

        /// <summary>Creates a new NSwagDocument.</summary>
        /// <returns>The document.</returns>
        public static NSwagDocument Create()
        {
            return Create<NSwagDocument>();
        }

        /// <summary>Loads an existing NSwagDocument.</summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The document.</returns>
        public static Task<NSwagDocument> LoadAsync(string filePath)
        {
            return LoadAsync<NSwagDocument>(filePath, null, false);
        }

        /// <summary>Loads an existing NSwagDocument with environment variable expansions and variables.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>The document.</returns>
        public static Task<NSwagDocument> LoadWithTransformationsAsync(string filePath, string variables)
        {
            return LoadAsync<NSwagDocument>(filePath, variables, true);
        }

        /// <summary>Executes the document.</summary>
        /// <returns>The task.</returns>
        public override async Task<OpenApiDocumentExecutionResult> ExecuteAsync()
        {
            var document = await GenerateSwaggerDocumentAsync();
            var tasks = new List<Task>();
            foreach (var codeGenerator in CodeGenerators.Items)
            {
                if (string.IsNullOrEmpty(codeGenerator.OutputFilePath))
                {
                    continue;
                }

                tasks.Add(Task.Run(async () =>
                {
                    await Task.Yield();

                    codeGenerator.Input = document;
                    await codeGenerator.RunAsync(null, null);
                    codeGenerator.Input = null;
                }));
            }

            await Task.WhenAll(tasks);

            return new OpenApiDocumentExecutionResult(null, null, true);
        }

        /// <summary>Executes the document via command line.</summary>
        /// <param name="redirectOutput">Indicates whether to redirect the outputs.</param>
        /// <returns>The result.</returns>
        public async Task<OpenApiDocumentExecutionResult> ExecuteCommandLineAsync(bool redirectOutput)
        {
            if (Runtime == Runtime.Debug)
            {
                return await ExecuteAsync();
            }

            var baseFilename = System.IO.Path.GetTempPath() + "nswag_document_" + Guid.NewGuid();
            var swaggerFilename = baseFilename + "_swagger.json";
            var filenames = new List<string>();

            var clone = FromJson<NSwagDocument>(null, ToJson());
            if (redirectOutput || string.IsNullOrEmpty(clone.SelectedSwaggerGenerator.OutputFilePath))
            {
                clone.SelectedSwaggerGenerator.OutputFilePath = swaggerFilename;
            }

            foreach (var command in clone.CodeGenerators.Items.Where(c => c != null))
            {
                if (redirectOutput || string.IsNullOrEmpty(command.OutputFilePath))
                {
                    var codeFilePath = baseFilename + "_" + command.GetType().Name + ".temp";
                    command.OutputFilePath = codeFilePath;
                    filenames.Add(codeFilePath);
                }
            }

            var configFilename = baseFilename + "_config.json";
            File.WriteAllText(configFilename, clone.ToJson());
            try
            {
                var command = "run \"" + configFilename + "\"";
                var output = await StartCommandLineProcessAsync(command);
                return clone.ProcessExecutionResult(output, baseFilename, redirectOutput);
            }
            finally
            {
                DeleteFileIfExists(configFilename);
                DeleteFileIfExists(swaggerFilename);
                foreach (var filename in filenames)
                {
                    DeleteFileIfExists(filename);
                }
            }
        }

        /// <summary>Converts to absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The absolute path.</returns>
        protected override string ConvertToAbsolutePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !System.IO.Path.IsPathRooted(pathToConvert) && !pathToConvert.Contains('%'))
            {
                return PathUtilities.MakeAbsolutePath(pathToConvert, GetDocumentDirectory());
            }

            return pathToConvert;
        }

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected override string ConvertToRelativePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !pathToConvert.Contains("C:\\Program Files\\") && !pathToConvert.Contains('%'))
            {
                return PathUtilities.MakeRelativePath(pathToConvert, GetDocumentDirectory())?.Replace("\\", "/");
            }

            return pathToConvert?.Replace("\\", "/");
        }

        private OpenApiDocumentExecutionResult ProcessExecutionResult(string output, string baseFilename, bool redirectOutput)
        {
            var swaggerOutput = ReadFileIfExists(SelectedSwaggerGenerator.OutputFilePath);
            var result = new OpenApiDocumentExecutionResult(output, swaggerOutput, redirectOutput);
            foreach (var command in CodeGenerators.Items.Where(c => c != null))
            {
                if (redirectOutput || string.IsNullOrEmpty(command.OutputFilePath))
                {
                    var codeFilepath = baseFilename + "_" + command.GetType().Name + ".temp";
                    result.AddGeneratorOutput(command.GetType(), ReadFileIfExists(codeFilepath));
                }
                else
                {
                    result.AddGeneratorOutput(command.GetType(), ReadFileIfExists(command.OutputFilePath));
                }
            }
            return result;
        }

        private async Task<string> StartCommandLineProcessAsync(string command)
        {
            var processStart = new ProcessStartInfo(GetProgramName(), GetArgumentsPrefix() + command);
            processStart.RedirectStandardOutput = true;
            processStart.RedirectStandardError = true;
            processStart.UseShellExecute = false;
            processStart.CreateNoWindow = true;

            var process = Process.Start(processStart);
            var output = await process.StandardOutput.ReadToEndAsync() +
                "\n\n" + await process.StandardError.ReadToEndAsync();

            if (process.ExitCode != 0)
            {
                var errorStart = output.IndexOf("...", StringComparison.Ordinal);
                if (errorStart < 0)
                {
                    errorStart = Regex.Match(output, "\n[^\n\r]*?Exception: .*", RegexOptions.Singleline)?.Index ?? -1;
                }

                var error = errorStart > 0 ? output.Substring(errorStart + 4) : output;
                var stackTraceStart = error.IndexOf("Server stack trace: ", StringComparison.Ordinal);
                if (stackTraceStart < 0)
                {
                    stackTraceStart = error.IndexOf("   at ", StringComparison.Ordinal);
                }

                var message = stackTraceStart > 0 ? error.Substring(0, stackTraceStart) : error;
                var stackTrace = stackTraceStart > 0 ? error.Substring(stackTraceStart) : "";

                if (message.Contains("Could not load type"))
                {
                    message += "Try running the document in another runtime, e.g. /runtime:NetCore20";
                }

                if (message.Contains("The system cannot find the file specified"))
                {
                    message += "Check if .NET Core is installed and 'dotnet' is globally available.";
                }

                throw new CommandLineException(message, "Runtime: " + Runtime + "\n" + stackTrace);
            }

            return output;
        }

        private string GetDocumentDirectory()
        {
            var absoluteDocumentPath = PathUtilities.MakeAbsolutePath(Path, Directory.GetCurrentDirectory());
            return System.IO.Path.GetDirectoryName(absoluteDocumentPath);
        }

#pragma warning disable CA1822
        private string GetArgumentsPrefix()
#pragma warning restore CA1822
        {
#if NET462

            var runtime = Runtime != Runtime.Default ? Runtime : RuntimeUtilities.CurrentRuntime;
            if (runtime == Runtime.Net80)
            {
                return "\"" + System.IO.Path.Combine(RootBinaryDirectory, "Net80/dotnet-nswag.dll") + "\" ";
            }
            if (runtime == Runtime.Net90)
            {
                return "\"" + System.IO.Path.Combine(RootBinaryDirectory, "Net90/dotnet-nswag.dll") + "\" ";
            }
#endif
            return "";
        }

#pragma warning disable CA1822
        private string GetProgramName()
#pragma warning restore CA1822
        {
#if NET462
            var runtime = Runtime != Runtime.Default ? Runtime : RuntimeUtilities.CurrentRuntime;
            if (runtime is Runtime.WinX64 or Runtime.Debug)
            {
                return System.IO.Path.Combine(RootBinaryDirectory, "Win/nswag.exe");
            }

            if (runtime == Runtime.WinX86)
            {
                return System.IO.Path.Combine(RootBinaryDirectory, "Win/nswag.x86.exe");
            }
#endif
            return "dotnet";
        }

        private static string ReadFileIfExists(string filename)
        {
            if (filename != null && File.Exists(filename))
            {
                return File.ReadAllText(filename);
            }

            return null;
        }

        private static void DeleteFileIfExists(string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }

        private sealed class CommandLineException : Exception
        {
            public CommandLineException(string message, string stackTrace)
                : base(message)
            {
                StackTrace = stackTrace;
            }

            public override string StackTrace { get; }

            public override string ToString()
            {
                return Message + "\n" + StackTrace;
            }
        }
    }
}
