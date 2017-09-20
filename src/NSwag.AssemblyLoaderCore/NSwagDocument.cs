//-----------------------------------------------------------------------
// <copyright file="NSwagDocument.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NSwag.SwaggerGeneration.Utilities;

namespace NSwag.Commands
{
    /// <summary>The NSwagDocument implementation.</summary>
    /// <seealso cref="NSwagDocumentBase" />
    public class NSwagDocument : NSwagDocumentBase
    {
        /// <summary>Gets or sets the root binary directory where the command line executables loaded from.</summary>
        public static string RootBinaryDirectory { get; set; } =
            System.IO.Path.GetDirectoryName(typeof(NSwagDocument).GetTypeInfo().Assembly.Location);

        /// <summary>Initializes a new instance of the <see cref="NSwagDocument"/> class.</summary>
        public NSwagDocument()
        {
            SwaggerGenerators.WebApiToSwaggerCommand = new WebApiToSwaggerCommand();
            SwaggerGenerators.AssemblyTypeToSwaggerCommand = new AssemblyTypeToSwaggerCommand();
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
            return LoadAsync<NSwagDocument>(filePath, new Dictionary<Type, Type>
            {
                { typeof(WebApiToSwaggerCommandBase), typeof(WebApiToSwaggerCommand) },
                { typeof(AssemblyTypeToSwaggerCommandBase), typeof(AssemblyTypeToSwaggerCommand) }
            });
        }

        /// <summary>Executes the document.</summary>
        /// <returns>The task.</returns>
        public override async Task<SwaggerDocumentExecutionResult> ExecuteAsync()
        {
            var document = await GenerateSwaggerDocumentAsync();
            foreach (var codeGenerator in CodeGenerators.Items.Where(c => !string.IsNullOrEmpty(c.OutputFilePath)))
            {
                codeGenerator.Input = document;
                await codeGenerator.RunAsync(null, null);
                codeGenerator.Input = null;
            }

            return new SwaggerDocumentExecutionResult(null, null, true);
        }

        /// <summary>Executes the document via command line.</summary>
        /// <param name="redirectOutput">Indicates whether to redirect the outputs.</param>
        /// <returns>The result.</returns>
        public async Task<SwaggerDocumentExecutionResult> ExecuteCommandLineAsync(bool redirectOutput)
        {
            return await Task.Run(async () =>
            {
                if (Runtime == Runtime.Debug)
                    return await ExecuteAsync();

                var baseFilename = System.IO.Path.GetTempPath() + "nswag_document_" + Guid.NewGuid();
                var swaggerFilename = baseFilename + "_swagger.json";
                var filenames = new List<string>();

                var clone = FromJson<NSwagDocument>(null, ToJson());
                if (redirectOutput || string.IsNullOrEmpty(clone.SelectedSwaggerGenerator.OutputFilePath))
                    clone.SelectedSwaggerGenerator.OutputFilePath = swaggerFilename;

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
                        DeleteFileIfExists(filename);
                }
            });
        }

        /// <summary>Gets the available controller types by calling the command line.</summary>
        /// <returns>The controller names.</returns>
        public async Task<string[]> GetControllersFromCommandLineAsync()
        {
            return await Task.Run(async () =>
            {
                if (!(SelectedSwaggerGenerator is WebApiToSwaggerCommand))
                    return new string[0];

                var baseFilename = System.IO.Path.GetTempPath() + "nswag_document_" + Guid.NewGuid();
                var configFilename = baseFilename + "_config.json";
                File.WriteAllText(configFilename, ToJson());
                try
                {
                    var command = "list-controllers /file:\"" + configFilename + "\"";
                    return GetListFromCommandLineOutput(await StartCommandLineProcessAsync(command));
                }
                finally
                {
                    DeleteFileIfExists(configFilename);
                }
            });
        }

        /// <summary>Gets the available controller types by calling the command line.</summary>
        /// <returns>The controller names.</returns>
        public async Task<string[]> GetTypesFromCommandLineAsync()
        {
            return await Task.Run(async () =>
            {
                if (!(SelectedSwaggerGenerator is AssemblyTypeToSwaggerCommand))
                    return new string[0];

                var baseFilename = System.IO.Path.GetTempPath() + "nswag_document_" + Guid.NewGuid();
                var configFilename = baseFilename + "_config.json";
                File.WriteAllText(configFilename, ToJson());
                try
                {
                    var command = "list-types /file:\"" + configFilename + "\"";
                    return GetListFromCommandLineOutput(await StartCommandLineProcessAsync(command));
                }
                finally
                {
                    DeleteFileIfExists(configFilename);
                }
            });
        }

        /// <summary>Converts to absolute path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The absolute path.</returns>
        protected override string ConvertToAbsolutePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !System.IO.Path.IsPathRooted(pathToConvert))
                return PathUtilities.MakeAbsolutePath(pathToConvert, GetDocumentDirectory());
            return pathToConvert;
        }

        /// <summary>Converts a path to an relative path.</summary>
        /// <param name="pathToConvert">The path to convert.</param>
        /// <returns>The relative path.</returns>
        protected override string ConvertToRelativePath(string pathToConvert)
        {
            if (!string.IsNullOrEmpty(pathToConvert) && !pathToConvert.Contains("C:\\Program Files\\"))
                return PathUtilities.MakeRelativePath(pathToConvert, GetDocumentDirectory())?.Replace("\\", "/");
            return pathToConvert?.Replace("\\", "/");
        }

        private string[] GetListFromCommandLineOutput(string output)
        {
            return output.Replace("\r\n", "\n")
                .Split(new string[] { "\n\n" }, StringSplitOptions.None)[1]
                .Split('\n')
                .Where(t => !string.IsNullOrEmpty(t))
                .ToArray();
        }

        private SwaggerDocumentExecutionResult ProcessExecutionResult(string output, string baseFilename, bool redirectOutput)
        {
            var swaggerOutput = ReadFileIfExists(SelectedSwaggerGenerator.OutputFilePath);
            var result = new SwaggerDocumentExecutionResult(output, swaggerOutput, redirectOutput);
            foreach (var command in CodeGenerators.Items.Where(c => c != null))
            {
                if (redirectOutput || string.IsNullOrEmpty(command.OutputFilePath))
                {
                    var codeFilepath = baseFilename + "_" + command.GetType().Name + ".temp";
                    result.AddGeneratorOutput(command.GetType(), ReadFileIfExists(codeFilepath));
                }
                else
                    result.AddGeneratorOutput(command.GetType(), ReadFileIfExists(command.OutputFilePath));
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
            var output = await process.StandardOutput.ReadToEndAsync();

            if (process.ExitCode != 0)
            {
                var error = await process.StandardError.ReadToEndAsync();
                if (error != null)
                    throw new InvalidOperationException(output + error);
            }

            return output;
        }

        private string GetDocumentDirectory()
        {
            var absoluteDocumentPath = PathUtilities.MakeAbsolutePath(Path, System.IO.Directory.GetCurrentDirectory());
            return System.IO.Path.GetDirectoryName(absoluteDocumentPath);
        }

        private string GetArgumentsPrefix()
        {
            if (Runtime == Runtime.NetCore10)
                return "\"" + System.IO.Path.Combine(RootBinaryDirectory, "NetCore10/dotnet-nswag.dll") + "\" ";
            else if (Runtime == Runtime.NetCore11)
                return "\"" + System.IO.Path.Combine(RootBinaryDirectory, "NetCore11/dotnet-nswag.dll") + "\" ";
            else if (Runtime == Runtime.NetCore20)
                return "\"" + System.IO.Path.Combine(RootBinaryDirectory, "NetCore20/dotnet-nswag.dll") + "\" ";
            else
                return "";
        }

        private string GetProgramName()
        {
            if (Runtime == Runtime.WinX64)
                return System.IO.Path.Combine(RootBinaryDirectory, "Win/nswag.exe");
            else if (Runtime == Runtime.WinX86)
                return System.IO.Path.Combine(RootBinaryDirectory, "Win/nswag.x86.exe");
            else
                return "dotnet";
        }

        private string ReadFileIfExists(string filename)
        {
            if (filename != null && File.Exists(filename))
                return File.ReadAllText(filename);
            return null;
        }

        private void DeleteFileIfExists(string filename)
        {
            if (File.Exists(filename))
                File.Delete(filename);
        }
    }
}