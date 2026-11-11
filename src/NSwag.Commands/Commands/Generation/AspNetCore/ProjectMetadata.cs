//-----------------------------------------------------------------------
// <copyright file="ProjectMetadata.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using NConsole;

namespace NSwag.Commands.Generation.AspNetCore
{
    [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
    internal sealed class ProjectMetadata
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
            string outputPath,
            IConsoleHost console = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(file), "file is null or empty.");

            var args = CreateMsBuildArguments(file, framework, configuration, runtime, noBuild, outputPath);

            var metadata = await TryReadingUsingGetProperties(args, file, noBuild);
            if (metadata == null || string.IsNullOrWhiteSpace(metadata[nameof(ProjectDir)]))
            {
                metadata = await ReadUsingMsBuildTargets(args, file, buildExtensionsDir, console);
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

            if (string.IsNullOrEmpty(outputPath))
            {
                projectMetadata.OutputPath = Path.Combine(projectMetadata.ProjectDir, projectMetadata.OutputPath);
            }

            projectMetadata.OutputPath = Path.GetFullPath(projectMetadata.OutputPath);

            return projectMetadata;
        }

        private static async Task<Dictionary<string, string>> ReadUsingMsBuildTargets(
            List<string> args,
            string file,
            string buildExtensionsDir,
            IConsoleHost console)
        {
            buildExtensionsDir ??= Path.Combine(Path.GetDirectoryName(file), "obj");

            Directory.CreateDirectory(buildExtensionsDir);

            var targetsPath = Path.Combine(buildExtensionsDir, Path.GetFileName(file) + ".NSwag.targets");
            var type = typeof(ProjectMetadata).GetTypeInfo();

            using (var input = type.Assembly.GetManifestResourceStream($"NSwag.Commands.Commands.Generation.AspNetCore.AspNetCore.targets"))
            using (var output = File.Open(targetsPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                // NB: Copy always in case it changes
                await input.CopyToAsync(output);
            }

            Dictionary<string, string> metadata;
            var metadataFile = Path.GetTempFileName();

            args.Add($"/t:{GetMetadataTarget}");
            args.Add($"/p:NSwagOutputMetadataFile={metadataFile}");

            try
            {
                var exitCode = await Exe.RunAsync("dotnet", args, console).ConfigureAwait(false);
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

                metadata = File.ReadLines(metadataFile).Select(l => l.Split([':'], 2))
                    .ToDictionary(s => s[0], s => s[1].TrimStart());
            }
            finally
            {
                File.Delete(metadataFile);
            }

            return metadata;
        }

        private static List<string> CreateMsBuildArguments(string file, string framework, string configuration, string runtime, bool noBuild, string outputPath)
        {
            var args = new List<string>
            {
                "msbuild",
                "/nologo",
                "/verbosity:quiet",
                // Worker nodes inherit the redirected pipes and outlive the msbuild process when
                // they are kept for reuse, so a reader waiting for end of stream never returns.
                "/nodeReuse:false"
            };

            if (!noBuild)
            {
                args.Add("/t:Build");
            }

            if (!string.IsNullOrEmpty(framework))
            {
                args.Add($"/p:TargetFramework={framework}");
            }

            if (!string.IsNullOrEmpty(configuration))
            {
                args.Add($"/p:Configuration={configuration}");
            }

            if (!string.IsNullOrEmpty(runtime))
            {
                args.Add($"/p:RuntimeIdentifier={runtime}");
            }

            if (!string.IsNullOrEmpty(outputPath))
            {
                args.Add($"/p:OutputPath={outputPath}");
            }

            if (!string.IsNullOrEmpty(file))
            {
                args.Add(file);
            }

            return args;
        }

        /// <summary>
        /// Kills the process and, where the runtime can, everything it spawned. MSBuild worker
        /// nodes are separate processes, so killing the driver alone leaves them building.
        /// </summary>
        private static void TryKillTree(Process process)
        {
            try
            {
#if NET462
                // No process-tree overload on .NET Framework; the worker nodes outlive this.
                process.Kill();
#else
                process.Kill(entireProcessTree: true);
#endif
            }
            catch (Exception)
            {
                // it exited between the wait timing out and the kill
            }
        }

        /// <summary>
        /// NET 8 and later support evaluating properties via CLI. https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#cli-based-project-evaluation
        /// </summary>
        private static async Task<Dictionary<string, string>> TryReadingUsingGetProperties(List<string> args, string file, bool noBuild)
        {
            // These arguments carry /t:Build when noBuild is false, so this is a full build of the
            // project and its references. Ten seconds was shorter than a cold build, which made
            // falling back the normal path and left an abandoned build writing the same obj
            // directory as the one the fallback starts. Waiting for it avoids that race and a
            // second build of the same project.
            const int BuildTimeoutMilliseconds = 5 * 60 * 1000;

            // Once the process has exited its output is already there. This only guards against a
            // worker node that inherited the pipes and is holding them open.
            const int ReadTimeoutMilliseconds = 30 * 1000;

            var properties = new[]
            {
                "BaseIntermediateOutputPath",
                "Platform",
                nameof(AssemblyName),
                nameof(OutputPath),
                nameof(PlatformTarget),
                nameof(ProjectDepsFilePath),
                nameof(ProjectDir),
                nameof(ProjectRuntimeConfigFilePath),
                nameof(TargetFileName),
                nameof(TargetFrameworkIdentifier),
                "MSBuildProjectDirectory",
            };

            try
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(file),
                    FileName = "dotnet",
                    Arguments = $"{string.Join(" ", args)} --getProperty:{string.Join(";", properties)}",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                });

                // Both pipes have to be drained while the process runs. Waiting for exit first
                // deadlocks as soon as the child fills a buffer, and stderr was never read at all.
                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                // When noBuild is false these arguments carry /t:Build, so this is a full build of
                // the project and its references and can legitimately outlast the timeout on a cold
                // machine, so this waits for it instead of cutting a build short. The wait runs off
                // the calling thread: NSwagStudio reaches this synchronously, and blocking there
                // would freeze its UI for the whole timeout.
                var exited = await Task.Run(() => process.WaitForExit(BuildTimeoutMilliseconds)).ConfigureAwait(false);
                if (!exited)
                {
                    // After five minutes nothing useful is coming. Leaving it alive would put a
                    // second MSBuild on the same obj directory as the fallback below, which
                    // MSBuild does not coordinate. Reading ExitCode would throw while it runs.
                    TryKillTree(process);
                    return null;
                }

                if (process.ExitCode != 0)
                {
                    return null;
                }

                {
                    // MSBuild worker nodes inherit the pipes and can outlive dotnet itself, so the
                    // read waits for an end of stream that never arrives. Bound it and fall back.
                    using var readTimeoutCts = new CancellationTokenSource();
                    var readTimeoutTask = Task.Delay(ReadTimeoutMilliseconds, readTimeoutCts.Token);
                    if (await Task.WhenAny(outputTask, readTimeoutTask).ConfigureAwait(false) == readTimeoutTask)
                    {
                        return null;
                    }

                    // Stop the timer as soon as the read finishes (CA2027).
                    readTimeoutCts.Cancel();

                    var output = await outputTask.ConfigureAwait(false);

                    var document = JsonSerializer.Deserialize<JsonDocument>(output);
                    var metadata = document.RootElement.GetProperty("Properties")
                        .EnumerateObject()
                        .ToDictionary(x => x.Name, x => x.Value.ToString().Trim());

                    // depending on the project type or MSBuild evaluation process, the ProjectDir property may not be set
                    if (string.IsNullOrWhiteSpace(metadata[nameof(ProjectDir)]))
                    {
                        metadata[nameof(ProjectDir)] = metadata["MSBuildProjectDirectory"];
                    }

                    return metadata;
                }
            }
            catch (Exception)
            {
                // ignore
            }

            return null;
        }
    }
}
