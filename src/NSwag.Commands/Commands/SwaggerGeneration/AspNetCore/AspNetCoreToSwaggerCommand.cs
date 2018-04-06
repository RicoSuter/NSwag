//-----------------------------------------------------------------------
// <copyright file="NSwagSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;
using Newtonsoft.Json;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    /// <summary>The generator.</summary>
    [Command(Name = "aspnetcore2swagger", Description = "Generates a Swagger specification ASP.NET Core Mvc application using ApiExplorer (experimental).")]
    public class AspNetCoreToSwaggerCommand : SwaggerGeneratorCommandBase<AspNetCoreToSwaggerGeneratorSettings>
    {
        private const string LauncherBinaryName = "NSwag.AspNetCore.Launcher";

        [Argument(Name = nameof(Project), IsRequired = false, Description = "The project to use.")]
        public string Project { get; set; }

        [Argument(Name = nameof(MSBuildProjectExtensionsPath), IsRequired = false, Description = "The MSBuild project extensions path. Defaults to \"obj\".")]
        public string MSBuildProjectExtensionsPath { get; set; }

        [Argument(Name = nameof(Configuration), IsRequired = false, Description = "The configuration to use.")]
        public string Configuration { get; set; }

        [Argument(Name = nameof(Runtime), IsRequired = false, Description = "The runtime to use.")]
        public string Runtime { get; set; }

        [Argument(Name = nameof(TargetFramework), IsRequired = false, Description = "The target framework to use.")]
        public string TargetFramework { get; set; }

        [Argument(Name = nameof(NoBuild), IsRequired = false, Description = "Don't build the project. Only use this when the build is up-to-date.")]
        public bool NoBuild { get; set; }

        [Argument(Name = nameof(Verbose), IsRequired = false, Description = "Print verbose output.")]
        public bool Verbose { get; set; }

        public override async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            // Run with .csproj
            if (!string.IsNullOrEmpty(Project))
            {
                var verboseHost = Verbose ? host : null;

                var projectFile = ProjectMetadata.FindProject(Project);
                var projectMetadata = await ProjectMetadata.GetProjectMetadata(
                    projectFile,
                    MSBuildProjectExtensionsPath,
                    TargetFramework,
                    Configuration,
                    Runtime,
                    NoBuild,
                    verboseHost).ConfigureAwait(false);

                if (!File.Exists(Path.Combine(projectMetadata.OutputPath, projectMetadata.TargetFileName)))
                {
                    throw new InvalidOperationException($"Project outputs could not be located in " +
                                                        $"'{projectMetadata.OutputPath}'. Ensure that the project has been built.");
                }

                var cleanupFiles = new List<string>();

                var toolDirectory = Path.GetDirectoryName(typeof(AspNetCoreToSwaggerCommand).GetTypeInfo().Assembly.Location);
                var args = new List<string>();
                string executable;

#if NET451
                if (projectMetadata.TargetFrameworkIdentifier == ".NETFramework")
                {
                    string binaryName;
                    var is32BitProject = string.Equals(projectMetadata.PlatformTarget, "x86", StringComparison.OrdinalIgnoreCase);
                    if (is32BitProject)
                    {
                        if (Environment.Is64BitProcess)
                            throw new InvalidOperationException($"The ouput of {projectFile} is a 32-bit application and requires NSwag.Console.x86 to be processed.");
                        binaryName = LauncherBinaryName + ".x86.exe";
                    }
                    else
                    {
                        if (!Environment.Is64BitProcess)
                            throw new InvalidOperationException($"The ouput of {projectFile} is a 64-bit application and requires NSwag.Console to be processed.");
                        binaryName = LauncherBinaryName + ".exe";
                    }

                    var executableSource = Path.Combine(toolDirectory, binaryName);
                    if (!File.Exists(executableSource))
                        throw new InvalidOperationException($"Unable to locate {binaryName} in {toolDirectory}.");

                    executable = Path.Combine(projectMetadata.OutputPath, binaryName);
                    File.Copy(executableSource, executable, overwrite: true);
                    cleanupFiles.Add(executable);

                    var appConfig = Path.Combine(projectMetadata.OutputPath, projectMetadata.TargetFileName + ".config");
                    if (File.Exists(appConfig))
                    {
                        var copiedAppConfig = Path.ChangeExtension(executable, ".exe.config");
                        File.Copy(appConfig, copiedAppConfig, overwrite: true);
                        cleanupFiles.Add(copiedAppConfig);
                    }
                }
#elif NETSTANDARD1_6
                if (projectMetadata.TargetFrameworkIdentifier == ".NETCoreApp")
                {
                    executable = "dotnet";
                    args.Add("exec");
                    args.Add("--depsfile");
                    args.Add(projectMetadata.ProjectDepsFilePath);

                    args.Add("--runtimeconfig");
                    args.Add(projectMetadata.ProjectRuntimeConfigFilePath);

                    var binaryName = LauncherBinaryName + ".dll";
                    var executorBinary = Path.Combine(toolDirectory, binaryName);
                    if (!File.Exists(executorBinary))
                    {
                        throw new InvalidOperationException($"Unable to locate {binaryName} in {toolDirectory}.");
                    }
                    args.Add(executorBinary);
                }
#endif
                else
                {
                    throw new InvalidOperationException($"Unsupported target framework '{projectMetadata.TargetFrameworkIdentifier}'.");
                }

                var commandFile = Path.GetTempFileName();
                var outputFile = Path.GetTempFileName();
                File.WriteAllText(commandFile, JsonConvert.SerializeObject(this));
                cleanupFiles.Add(commandFile);
                cleanupFiles.Add(outputFile);

                args.Add(commandFile);
                args.Add(outputFile);
                args.Add(projectMetadata.AssemblyName);
                args.Add(toolDirectory);
                try
                {
                    var exitCode = await Exe.RunAsync(executable, args, verboseHost).ConfigureAwait(false);
                    if (exitCode != 0)
                        throw new InvalidOperationException($"Swagger generation failed with non-zero exit code '{exitCode}'.");

                    host?.WriteMessage($"Output written to {outputFile}.{Environment.NewLine}");

                    var documentJson = File.ReadAllText(outputFile);
                    var document = await SwaggerDocument.FromJsonAsync(documentJson, expectedSchemaType: OutputType).ConfigureAwait(false);
                    await this.TryWriteDocumentOutputAsync(host, () => document).ConfigureAwait(false);
                    return document;
                }
                finally
                {
                    TryDeleteFiles(cleanupFiles);
                }
            }
            else
            {
                return await base.RunAsync(processor, host);
            }
        }

        protected override async Task<string> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            // Run with .dll

            Settings.DocumentTemplate = await GetDocumentTemplateAsync();
            InitializeCustomTypes(assemblyLoader);

            // TODO: Load TestServer, get ApiExplorer, run generator

            //var generator = new AspNetCoreToSwaggerGenerator(Settings);
            //var document = await generator.GenerateAsync(controllerTypes).ConfigureAwait(false);

            //return PostprocessDocument(document);

            return null;
        }

        private static void TryDeleteFiles(List<string> files)
        {
            foreach (var file in files)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch
                {
                    // Don't throw any if any clean up operation fails.
                }
            }
        }
    }
}
