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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using NConsole;
using Newtonsoft.Json;
using NSwag.SwaggerGeneration.AspNetCore;
using NJsonSchema.Yaml;
using NJsonSchema;

#if NETCOREAPP || NETSTANDARD
using System.Runtime.Loader;
#endif

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
        public bool Verbose { get; set; } = true;

        [Argument(Name = nameof(WorkingDirectory), IsRequired = false, Description = "The working directory to use.")]
        public string WorkingDirectory { get; set; }

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

                var args = new List<string>();
                string executable;

#if NET451
                var toolDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(toolDirectory))
                    toolDirectory = Path.GetDirectoryName(typeof(AspNetCoreToSwaggerCommand).GetTypeInfo().Assembly.Location);

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
#elif NETCOREAPP || NETSTANDARD
                var toolDirectory = AppContext.BaseDirectory;
                if (!Directory.Exists(toolDirectory))
                    toolDirectory = Path.GetDirectoryName(typeof(AspNetCoreToSwaggerCommand).GetTypeInfo().Assembly.Location);

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

                    JsonReferenceResolver ReferenceResolverFactory(SwaggerDocument d) => new JsonAndYamlReferenceResolver(new JsonSchemaResolver(d, Settings));

                    var documentJson = File.ReadAllText(outputFile);
                    var document = await SwaggerDocument.FromJsonAsync(documentJson, null, OutputType, ReferenceResolverFactory).ConfigureAwait(false);
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

        public string ChangeWorkingDirectory()
        {
            var currentWorkingDirectory = Directory.GetCurrentDirectory();

            if (!string.IsNullOrEmpty(WorkingDirectory))
            {
                Directory.SetCurrentDirectory(WorkingDirectory);
            }
            else if (!string.IsNullOrEmpty(Project))
            {
                var workingDirectory = Path.GetDirectoryName(Project);
                if (Directory.Exists(workingDirectory))
                {
                    Directory.SetCurrentDirectory(workingDirectory);
                }
            }
            else if (AssemblyPaths.Any())
            {
                var workingDirectory = Path.GetDirectoryName(AssemblyPaths.First());
                if (Directory.Exists(workingDirectory))
                {
                    Directory.SetCurrentDirectory(workingDirectory);
                }
            }

            return currentWorkingDirectory;
        }

        protected override async Task<string> RunIsolatedAsync(AssemblyLoader.AssemblyLoader assemblyLoader)
        {
            InitializeCustomTypes(assemblyLoader);

            var startupType = await GetStartupTypeAsync(assemblyLoader);
            var currentWorkingDirectory = ChangeWorkingDirectory();
            using (var testServer = CreateTestServer(startupType))
            {
                Directory.SetCurrentDirectory(currentWorkingDirectory);

                // See https://github.com/aspnet/Mvc/issues/5690
                var type = typeof(IApiDescriptionGroupCollectionProvider);
                var apiDescriptionProvider = (IApiDescriptionGroupCollectionProvider)testServer.Host.Services.GetRequiredService(type);

                var settings = await CreateSettingsAsync(assemblyLoader, testServer.Host, currentWorkingDirectory);
                var generator = new AspNetCoreToSwaggerGenerator(settings);
                var document = await generator.GenerateAsync(apiDescriptionProvider.ApiDescriptionGroups).ConfigureAwait(false);

                var json = PostprocessDocument(document);
                return json;
            }
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
