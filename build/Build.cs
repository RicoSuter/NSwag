using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using Project = Nuke.Common.ProjectModel.Project;

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

    AbsolutePath NSwagStudioBinaries => ArtifactsDirectory / "bin" / "NSwagStudio" / Configuration;
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
            Serilog.Log.Information($"Tag version {VersionPrefix} from Git found, using it as version prefix", versionPrefix);
        }
        else
        {
            var propsDocument = XDocument.Parse((RootDirectory / "Directory.Build.props").ReadAllText());
            versionPrefix = propsDocument.Element("Project").Element("PropertyGroup").Element("VersionPrefix").Value;
            Serilog.Log.Information("Version prefix {VersionPrefix} read from Directory.Build.props", versionPrefix);
        }

        return versionPrefix;
    }

    protected override void OnBuildInitialized()
    {
        VersionPrefix = DetermineVersionPrefix();

        var versionParts = VersionPrefix.Split('-');
        if (versionParts.Length == 2)
        {
            VersionPrefix = versionParts[0];
            VersionSuffix = versionParts[1];
        }
        else
        {
            VersionSuffix = !IsTaggedBuild
                ? $"preview-{DateTime.UtcNow:yyyyMMdd-HHmm}"
                : "";
        }

        if (IsLocalBuild)
        {
            VersionSuffix = $"dev-{DateTime.UtcNow:yyyyMMdd-HHmm}";
        }

        Serilog.Log.Information("BUILD SETUP");
        Serilog.Log.Information("Configuration:\t{Configuration}", Configuration);
        Serilog.Log.Information("Version prefix:\t{VersionPrefix}", VersionPrefix);
        Serilog.Log.Information("Version suffix:\t{VersionSuffix}",VersionSuffix);
        Serilog.Log.Information("Tagged build:\t{IsTaggedBuild}", IsTaggedBuild);
    }

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
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

    Target Restore => _ => _
        .Executes(() =>
        {
            NpmInstall(x => x
                .SetProcessWorkingDirectory(SourceDirectory / "NSwag.Npm")
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
            (SourceDirectory / "NSwag.Npm" / "bin" / "binaries").CreateOrCleanDirectory();
            NSwagStudioBinaries.CreateOrCleanDirectory();

            Serilog.Log.Information("Build and copy full .NET command line with configuration {Configuration}", Configuration);

            // TODO: Fix build here
            MSBuild(x => x
                .SetProjectFile(GetProject("NSwagStudio"))
                .SetTargets("Build")
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

            MSBuild(x => x
                .SetTargetPath(Solution)
                .SetTargets("Build")
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

            // later steps need to have binaries in correct places
            PublishAndCopyConsoleProjects();
        });

    Target Test => _ => _
        .After(Compile)
        .Executes(() =>
        {
            foreach (var project in Solution.AllProjects.Where(p => p.Name.EndsWith(".Tests")))
            {
                DotNetTest(x => x
                    .SetProjectFile(project)
                    .EnableNoRestore()
                    .SetConfiguration(Configuration)
                );
            }
        });

    void PublishAndCopyConsoleProjects()
    {
        var consoleCoreProject = GetProject("NSwag.ConsoleCore");
        var consoleX86Project = GetProject("NSwag.Console.x86");
        var consoleProject = GetProject("NSwag.Console");

        Serilog.Log.Information("Publish command line projects");

        void PublishConsoleProject(Project project, string[] targetFrameworks)
        {
            foreach (var targetFramework in targetFrameworks)
            {
                DotNetPublish(s => s
                    .SetProject(project)
                    .SetFramework(targetFramework)
                    .SetAssemblyVersion(VersionPrefix)
                    .SetFileVersion(VersionPrefix)
                    .SetInformationalVersion(VersionPrefix)
                    .SetConfiguration(Configuration)
                    .SetDeterministic(IsServerBuild)
                    .SetContinuousIntegrationBuild(IsServerBuild)
                );
            }
        }

        PublishConsoleProject(consoleX86Project, new[] { "net462" });
        PublishConsoleProject(consoleProject, new[] { "net462" });
        PublishConsoleProject(consoleCoreProject, new[] { "net6.0", "net7.0", "net8.0" });

        void CopyConsoleBinaries(AbsolutePath target)
        {
            // take just exe from X86 as other files are shared with console project
            var consoleX86Directory = ArtifactsDirectory / "publish" / consoleX86Project.Name / Configuration;
            CopyFileToDirectory(consoleX86Directory / "NSwag.x86.exe", target / "Win");
            CopyFileToDirectory(consoleX86Directory / "NSwag.x86.exe.config", target / "Win");

            CopyDirectoryRecursively(ArtifactsDirectory / "publish" / consoleProject.Name / Configuration, target / "Win", DirectoryExistsPolicy.Merge);

            CopyDirectoryRecursively(ArtifactsDirectory / "publish" / consoleCoreProject.Name / (Configuration + "_net6.0"), target / "Net60");
            CopyDirectoryRecursively(ArtifactsDirectory / "publish" / consoleCoreProject.Name / (Configuration + "_net7.0"), target / "Net70");
            CopyDirectoryRecursively(ArtifactsDirectory / "publish" / consoleCoreProject.Name / (Configuration + "_net8.0"), target / "Net80");
        }

        Serilog.Log.Information("Copy published Console for NSwagStudio");

        CopyConsoleBinaries(target: NSwagStudioBinaries);

        Serilog.Log.Information("Copy published Console for NPM");

        CopyConsoleBinaries(target: SourceDirectory / "NSwag.Npm" / "bin" / "binaries");
    }

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

    // Solution.GetProject only returns solution's direct descendants since NUKE 7.0.1
    private Project GetProject(string projectName) =>
        Solution.AllProjects.FirstOrDefault(x => x.Name == projectName) ?? throw new ArgumentException($"Could not find project {projectName} from solution");
}
