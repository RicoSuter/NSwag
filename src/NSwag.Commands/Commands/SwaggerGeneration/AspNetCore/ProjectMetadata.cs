// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NConsole;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    internal class ProjectMetadata
    {
        private const string GetMetadataTarget = "__GetNSwagProjectMetadata";

        private readonly string _file;
        private readonly string _framework;
        private readonly string _configuration;
        private readonly string _runtime;

        public ProjectMetadata(string file, string framework, string configuration, string runtime)
        {
            _file = file;
            _framework = framework;
            _configuration = configuration;
            _runtime = runtime;
            ProjectName = Path.GetFileName(file);
        }

        public string ProjectName { get; }

        public string AssemblyName { get; set; }
        public string OutputPath { get; set; }
        public string PlatformTarget { get; set; }
        public string ProjectDepsFilePath { get; set; }
        public string ProjectDir { get; set; }
        public string ProjectRuntimeConfigFilePath { get; set; }
        public string TargetFileName { get; set; }
        public string TargetFrameworkIdentifier { get; set; }

        public static string FindProject(string path)
        {
            if (path == null)
            {
                path = Directory.GetCurrentDirectory();
            }
            else if (!Directory.Exists(path))
            {
                // It's not a directory. Treat the input path as the project file.
                return path;
            }

            var projectFiles = Directory.GetFiles(path, "*.csproj");
            if (projectFiles.Length == 0)
            {
                throw new InvalidOperationException($"No project (.csproj) file could be found under directory {path}.");
            }
            else if (projectFiles.Length > 1)
            {
                throw new InvalidOperationException($"More than one project was found in directory '{path}'. Specify one using its file name.");
            }

            return projectFiles[0];
        }

        public static async Task<ProjectMetadata> GetProjectMetadata(
            string file,
            string buildExtensionsDir,
            string framework,
            string configuration,
            string runtime,
            bool noBuild,
            IConsoleHost console = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(file), "file is null or empty.");

            if (buildExtensionsDir == null)
            {
                buildExtensionsDir = Path.Combine(Path.GetDirectoryName(file), "obj");
            }

            Directory.CreateDirectory(buildExtensionsDir);

            var targetsPath = Path.Combine(
                buildExtensionsDir,
                Path.GetFileName(file) + ".NSwag.targets");
            var type = typeof(ProjectMetadata).GetTypeInfo();

            using (var input = type.Assembly.GetManifestResourceStream($"NSwag.Commands.Commands.SwaggerGeneration.AspNetCore.AspNetCore.targets"))
            using (var output = File.Open(targetsPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                // NB: Copy always in case it changes
                input.CopyTo(output);
            }

            IDictionary<string, string> metadata;
            var metadataFile = Path.GetTempFileName();
            try
            {
                var args = new List<string>
                {
                    "msbuild",
                    "/nologo",
                    "/verbosity:quiet",
                    "/p:NSwagOutputMetadataFile=" + metadataFile
                };

                if (!noBuild)
                {
                    args.Add("/t:Build");
                }

                args.Add("/t:" + GetMetadataTarget);

                if (framework != null)
                {
                    args.Add("/p:TargetFramework=" + framework);
                }
                if (configuration != null)
                {
                    args.Add("/p:Configuration=" + configuration);
                }
                if (runtime != null)
                {
                    args.Add("/p:RuntimeIdentifier=" + runtime);
                }

                if (file != null)
                {
                    args.Add(file);
                }

                var exitCode = await Exe.RunAsync("dotnet", args, console: console).ConfigureAwait(false);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Unable to retrieve project metadata. Ensure it's an MSBuild-based .NET Core project."
                        + "If you're using custom BaseIntermediateOutputPath or MSBuildProjectExtensionsPath values, Use the --msbuildprojectextensionspath option.");
                }

                if (console != null)
                {
                    console.WriteMessage("Done executing command" + Environment.NewLine);
                    console.WriteMessage("Output:" + Environment.NewLine + File.ReadAllText(metadataFile));
                }

                metadata = File.ReadLines(metadataFile).Select(l => l.Split(new[] { ':' }, 2))
                    .ToDictionary(s => s[0], s => s[1].TrimStart());
            }
            finally
            {
                File.Delete(metadataFile);
            }

            var platformTarget = metadata[nameof(PlatformTarget)];
            if (platformTarget.Length == 0)
            {
                platformTarget = metadata["Platform"];
            }

            var projectMetadata = new ProjectMetadata(file, framework, configuration, runtime)
            {
                AssemblyName = metadata[nameof(AssemblyName)],
                OutputPath = metadata[nameof(OutputPath)],
                PlatformTarget = platformTarget,
                ProjectDepsFilePath = metadata[nameof(ProjectDepsFilePath)],
                ProjectDir = metadata[nameof(ProjectDir)],
                ProjectRuntimeConfigFilePath = metadata[nameof(ProjectRuntimeConfigFilePath)],
                TargetFileName = metadata[nameof(TargetFileName)],
                TargetFrameworkIdentifier = metadata[nameof(TargetFrameworkIdentifier)],
            };

            projectMetadata.ProjectDir = Path.GetFullPath(projectMetadata.ProjectDir);
            projectMetadata.OutputPath = Path.GetFullPath(Path.Combine(projectMetadata.ProjectDir, projectMetadata.OutputPath));

            return projectMetadata;
        }
    }
}