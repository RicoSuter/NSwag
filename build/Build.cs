using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.VSTest;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Logger;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Tools.VSTest.VSTestTasks;
using Project = Nuke.Common.ProjectModel.Project;

[CheckBuildProjectConfigurations]
partial class Build : NukeBuild
{
    public Build()
    {
        var msBuildExtensionPath = Environment.GetEnvironmentVariable("MSBuildExtensionsPath");
        var msBuildExePath = Environment.GetEnvironmentVariable("MSBUILD_EXE_PATH");
        var msBuildSdkPath = Environment.GetEnvironmentVariable("MSBuildSDKsPath");

        MSBuildLocator.RegisterDefaults();
        TriggerAssemblyResolution();

        Environment.SetEnvironmentVariable("MSBuildExtensionsPath", msBuildExtensionPath);
        Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", msBuildExePath);
        Environment.SetEnvironmentVariable("MSBuildSDKsPath", msBuildSdkPath);
    }

    static void TriggerAssemblyResolution() => _ = new ProjectCollection();

    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    AbsolutePath NSwagStudioBinaries => SourceDirectory / "NSwagStudio" / "bin" / Configuration;
    AbsolutePath NSwagNpmBinaries => SourceDirectory / "NSwag.Npm";

    static bool IsRunningOnWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    bool IsTaggedBuild;
    string VersionPrefix;
    string VersionSuffix;

    string DetermineVersionPrefix()
    {
        var versionPrefix = GitRepository.Tags.SingleOrDefault(x => x.StartsWith("v"))?[1..];
        if (!string.IsNullOrWhiteSpace(versionPrefix))
        {
            IsTaggedBuild = true;
            Info($"Tag version {versionPrefix} from Git found, using it as version prefix");
        }
        else
        {
            var propsDocument = XDocument.Parse(TextTasks.ReadAllText(SourceDirectory / "Directory.Build.props"));
            versionPrefix = propsDocument.Element("Project").Element("PropertyGroup").Element("VersionPrefix").Value;
            Info($"Version prefix {versionPrefix} read from Directory.Build.props");
        }

        return versionPrefix;
    }

    protected override void OnBuildInitialized()
    {
        VersionPrefix = DetermineVersionPrefix();

        VersionSuffix = !IsTaggedBuild
            ? $"preview-{DateTime.UtcNow:yyyyMMdd-HHmm}"
            : "";

        if (IsLocalBuild)
        {
            VersionSuffix = $"dev-{DateTime.UtcNow:yyyyMMdd-HHmm}";
        }

        using var _ = Block("BUILD SETUP");
        Info("Configuration:\t" + Configuration);
        Info("Version prefix:\t" + VersionPrefix);
        Info("Version suffix:\t" + VersionSuffix);
        Info("Tagged build:\t" + IsTaggedBuild);
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });


    Target InstallDependencies => _ => _
        .Before(Restore, Compile)
        .Executes(() =>
        {
            Chocolatey("install wixtoolset -y");
            NpmInstall(x => x
                .EnableGlobal()
                .AddPackages("dotnettools")
            );
        });

    // logic from 00_Install.bat
    Target Restore => _ => _
        .Executes(() =>
        {
            NpmInstall(x => x
                .SetProcessWorkingDirectory(SourceDirectory / "NSwag.Npm")
            );

            NpmInstall(x => x
                .SetProcessWorkingDirectory(SourceDirectory / "NSwag.Integration.TypeScriptWeb")
            );

            MSBuild(x => x
                .SetTargetPath(Solution)
                .SetTargets("Restore")
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild)
                .SetVerbosity(MSBuildVerbosity.Minimal)
            );

            DotNetRestore(x => x
                .SetProjectFile(Solution)
                .SetVerbosity(DotNetVerbosity.Minimal)
            );
        });

    // logic from 01_Build.bat
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            EnsureCleanDirectory(SourceDirectory / "NSwag.Npm" / "bin" / "binaries");

            Info("Build and copy full .NET command line with configuration " + Configuration);

            MSBuild(x => x
                    .SetTargetPath(Solution)
                    .SetTargets("Rebuild")
                    .SetAssemblyVersion(VersionPrefix)
                    .SetFileVersion(VersionPrefix)
                    .SetInformationalVersion(VersionPrefix)
                    .SetConfiguration(Configuration)
                    .SetMaxCpuCount(Environment.ProcessorCount)
                    .SetNodeReuse(IsLocalBuild)
                    .SetVerbosity(MSBuildVerbosity.Minimal)
                    .SetProperty("Deterministic", IsServerBuild)
                    .SetProperty("ContinuousIntegrationBuild", IsServerBuild)
            );
        });

    // logic from 02_RunUnitTests.bat
    Target UnitTest => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var webApiTest = Solution.GetProject("NSwag.Generation.WebApi.Tests");
            VSTest(x => x
                .SetTestAssemblies(webApiTest.Directory / "bin" / Configuration / "NSwag.Generation.WebApi.Tests.dll")
            );

            /*
            VSTest(x => x
                .SetLogger(logger)
                .SetTestAssemblies(SourceDirectory / "NSwag.Tests" / "bin" / Configuration / "NSwag.Tests.dll")
            );
            */

            // project name + target framework pairs
            var dotNetTestTargets = new (string ProjectName, string Framework)[]
            {
                ("NSwag.CodeGeneration.Tests", null),
                ("NSwag.CodeGeneration.CSharp.Tests", null),
                ("NSwag.CodeGeneration.TypeScript.Tests", null),
                ("NSwag.Generation.AspNetCore.Tests", null),
                ("NSwag.Core.Tests", null),
                ("NSwag.Core.Yaml.Tests", null),
                ("NSwag.AssemblyLoader.Tests", null)
            };

            foreach (var (project, targetFramework) in dotNetTestTargets)
            {
                DotNetTest(x => x
                    .SetProjectFile(Solution.GetProject(project))
                    .SetFramework(targetFramework)
                    .SetConfiguration(Configuration)
                );
            }
        });

    // logic from 03_RunIntegrationTests.bat
    Target IntegrationTest => _ => _
        .After(Compile)
        .DependsOn(Samples)
        .Executes(() =>
        {
            var nswagCommand = NSwagStudioBinaries / "nswag.cmd";

            // project name + runtime pairs
            var dotnetTargets = new[]
            {
                ("NSwag.Sample.NETCore21", "NetCore21"),
                ("NSwag.Sample.NETCore31", "NetCore31"),
                ("NSwag.Sample.NET50", "Net50"),
                ("NSwag.Sample.NET60", "Net60"),
                ("NSwag.Sample.NET60Minimal", "Net60")
            };

            foreach (var (projectName, runtime) in dotnetTargets)
            {
                var project = Solution.GetProject(projectName);
                DotNetBuild(x => BuildDefaults(x)
                    .SetProcessWorkingDirectory(project.Directory)
                    .SetProperty("CopyLocalLockFileAssemblies", true)
                );
                var process = StartProcess(nswagCommand, $"run /runtime:{runtime}", workingDirectory: project.Directory);
                process.WaitForExit();
            }

            // project name + runtime pairs
            var msbuildTargets = new[]
            {
                ("NSwag.Sample.NetGlobalAsax", "Winx64"),
                ("NSwag.Integration.WebAPI", "Winx64")
            };

            foreach (var (projectName, runtime) in msbuildTargets)
            {
                var project = Solution.GetProject(projectName);
                MSBuild(x => x
                    .SetProcessWorkingDirectory(project.Directory)
                    .SetNodeReuse(IsLocalBuild)
                );
                var process = StartProcess(nswagCommand, $"run /runtime:{runtime}", workingDirectory: project.Directory);
                process.WaitForExit();
            }
        });

    Target Test => _ => _
        .DependsOn(UnitTest, IntegrationTest);

    // logic from runs.ps1
    Target Samples => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var studioProject = Solution.GetProject("NSwagStudio");

            void NSwagRun(
                Project project,
                string configurationFile,
                string runtime,
                Configuration configuration,
                bool build)
            {
                var nswagConfigurationFile = project.Directory / $"{configurationFile}.nswag";
                var nswagSwaggerFile = project.Directory / $"{configurationFile}_swagger.json";

                DeleteFile(nswagSwaggerFile);

                if (build)
                {
                    DotNetBuild(x => BuildDefaults(x)
                        .SetConfiguration(configuration)
                        .SetProjectFile(project)
                    );
                }
                else
                {
                    DotNetRestore(x => x
                        .SetProjectFile(project)
                    );
                }

                var cliPath = studioProject.Directory / "bin" / Configuration / runtime / "dotnet-nswag.dll";
                DotNet($"{cliPath} run {nswagConfigurationFile} /variables:configuration=" + configuration);

                if (!File.Exists(nswagSwaggerFile))
                {
                    throw new Exception($"Output ${nswagSwaggerFile} not generated for {nswagConfigurationFile}.");
                }
            }

            var samplesPath = RootDirectory / "samples";
            var sampleSolution = ProjectModelTasks.ParseSolution(samplesPath / "Samples.sln");
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_assembly", "NetCore21", Configuration.Release, true);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_project", "NetCore21", Configuration.Release, false);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_reflection", "NetCore21", Configuration.Release, true);

            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_assembly", "NetCore21", Configuration.Debug, true);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_project", "NetCore21", Configuration.Debug, false);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_reflection", "NetCore21", Configuration.Debug, true);
        });

    DotNetBuildSettings BuildDefaults(DotNetBuildSettings s)
    {
        return s
            .SetAssemblyVersion(VersionPrefix)
            .SetFileVersion(VersionPrefix)
            .SetInformationalVersion(VersionPrefix)
            .SetConfiguration(Configuration)
            .SetDeterministic(IsServerBuild)
            .SetContinuousIntegrationBuild(IsServerBuild);
    }
}
