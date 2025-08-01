using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
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

    // the file we want to build, can be either full solution on Windows or a filtered one on other platforms
    AbsolutePath SolutionFile;

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
        SolutionFile = IsRunningOnWindows ? Solution.Path : SourceDirectory / "NSwag.NoInstaller.slnf";

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

    Target Restore => _ => _
        .Executes(() =>
        {
            NpmInstall(x => x
                .SetProcessWorkingDirectory(SourceDirectory / "NSwag.Npm")
            );

            NpmInstall(x => x
                .SetProcessWorkingDirectory(GetProject("NSwag.CodeGeneration.TypeScript.Tests").Directory)
            );

            DotNetRestore(x => x
                .SetProjectFile(SolutionFile)
                .SetVerbosity(DotNetVerbosity.minimal)
                .AddProperty("BuildWithNetFrameworkHostedCompiler", "true")
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

            DotNetBuild(x => x
                .SetProjectFile(SolutionFile)
                .SetAssemblyVersion(VersionPrefix)
                .SetFileVersion(VersionPrefix)
                .SetInformationalVersion(VersionPrefix)
                .SetConfiguration(Configuration)
                .SetVerbosity(DotNetVerbosity.minimal)
                .SetDeterministic(IsServerBuild)
                .SetContinuousIntegrationBuild(IsServerBuild)
                // ensure we don't generate too much output in CI run
                // 0  Turns off emission of all warning messages
                // 1  Displays severe warning messages
                .SetWarningLevel(IsServerBuild ? 0 : 1)
                .EnableNoRestore()
            );

            // later steps need to have binaries in correct places
            PublishAndCopyConsoleProjects();
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            foreach (var project in Solution.AllProjects.Where(p => p.Name.EndsWith(".Tests")))
            {
                DotNetTest(x => x
                    .SetProjectFile(project)
                    .EnableNoRestore()
                    .EnableNoBuild()
                    .SetConfiguration(Configuration)
                    .When(GitHubActions.Instance is not null, x => x.SetLoggers("GitHubActions"))
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
                    // ensure we don't generate too much output in CI run
                    // 0  Turns off emission of all warning messages
                    // 1  Displays severe warning messages
                    .SetWarningLevel(IsServerBuild ? 0 : 1)
                    .EnableNoRestore()
                    .EnableNoBuild()
                );
            }
        }

        if (IsRunningOnWindows)
        {
            PublishConsoleProject(consoleX86Project, ["net462"]);
            PublishConsoleProject(consoleProject, ["net462"]);
        }
        PublishConsoleProject(consoleCoreProject, ["net8.0", "net9.0"]);

        void CopyConsoleBinaries(AbsolutePath target)
        {
            // take just exe from X86 as other files are shared with console project
            var configuration = Configuration.ToString().ToLowerInvariant();

            if (IsRunningOnWindows)
            {
                var consoleX86Directory = ArtifactsDirectory / "publish" / consoleX86Project.Name / configuration;
                (consoleX86Directory / "NSwag.x86.exe").CopyToDirectory(target / "Win");
                (consoleX86Directory / "NSwag.x86.exe.config").CopyToDirectory(target / "Win");

                (ArtifactsDirectory / "publish" / consoleProject.Name / configuration).Copy(target / "Win", ExistsPolicy.DirectoryMerge);
            }

            (ArtifactsDirectory / "publish" / consoleCoreProject.Name / (configuration + "_net8.0")).Copy(target / "Net80");
            (ArtifactsDirectory / "publish" / consoleCoreProject.Name / (configuration + "_net9.0")).Copy(target / "Net90");
        }

        if (IsRunningOnWindows)
        {
            Serilog.Log.Information("Copy published Console for NSwagStudio");
            CopyConsoleBinaries(target: NSwagStudioBinaries);
        }

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
