// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NSwag.Commands.SwaggerGeneration.AspNetCore
{
    internal class ProjectMetadata
    {
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
        public string NSwagExecutorDirectory { get; set; }

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

        public static ProjectMetadata GetProjectMetadata(
            string file,
            string buildExtensionsDir,
            string framework = null,
            string configuration = null,
            string runtime = null)
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

            using (var input = type.Assembly.GetManifestResourceStream($"{type.Namespace}.AspNetCore.targets"))
            using (var output = File.Open(targetsPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                // NB: Copy always in case it changes
                input.CopyTo(output);
            }

            IDictionary<string, string> metadata;
            var metadataFile = Path.GetTempFileName();
            try
            {
                var propertyArg = "/p:NSwagOutputMetadataFile=" + metadataFile;
                if (framework != null)
                {
                    propertyArg += " /p:TargetFramework=" + framework;
                }
                if (configuration != null)
                {
                    propertyArg += " /p:Configuration=" + configuration;
                }
                if (runtime != null)
                {
                    propertyArg += " /p:RuntimeIdentifier=" + runtime;
                }

                var args = new List<string>
                {
                    "msbuild",
                    "/target:__GetNSwagProjectMetadata",
                    propertyArg,
                    "/verbosity:quiet",
                    "/nologo",
                };

                if (file != null)
                {
                    args.Add(file);
                }

                var exitCode = Exe.Run("dotnet", args);
                if (exitCode != 0)
                {
                    throw new InvalidOperationException("Unable to retrieve project metadata. Ensure it's an MSBuild-based .NET Core project."
                        + "If you're using custom BaseIntermediateOutputPath or MSBuildProjectExtensionsPath values, Use the --msbuildprojectextensionspath option.");
                }

                Console.WriteLine(File.ReadAllText(metadataFile));

                metadata = File.ReadLines(metadataFile).Select(l => l.Split(new[] { ':' }, 2))
                    .ToDictionary(s => s[0], s => s[1].TrimStart());
            }
            finally
            {
                File.Delete(metadataFile);
            }

            var platformTarget = metadata["PlatformTarget"];
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

            if (metadata.TryGetValue(nameof(NSwagExecutorDirectory), out var value))
            {
                projectMetadata.NSwagExecutorDirectory = value;
            }

            return projectMetadata;
        }
    }
}