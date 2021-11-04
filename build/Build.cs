using System;
using System.IO;
using Nuke.Common;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.Npm;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Tools.VSTest;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Logger;
using static Nuke.Common.Tooling.ProcessTasks;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static Nuke.Common.Tools.VSTest.VSTestTasks;

[CheckBuildProjectConfigurations]
partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });


    Target InstallDependencies => _ => _
        .Before(Compile)
        .Executes(() =>
        {
            Chocolatey("install wixtoolset");
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

            Info("Build and copy full .NET command line");

            MSBuild(x => x
                    .SetTargetPath(Solution)
                    .SetTargets("Rebuild")
                    .SetConfiguration(Configuration)
                    .SetMaxCpuCount(Environment.ProcessorCount)
                    .SetNodeReuse(IsLocalBuild)
                    .SetVerbosity(MSBuildVerbosity.Minimal)
            );
        });


    // logic from 01_Build.bat
    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            if (Configuration != Configuration.Release)
            {
                throw new InvalidOperationException("Cannot pack if compilation hasn't been done in Release mode");
            }

            var npmBinariesDirectory = SourceDirectory / "NSwag.Npm" / "bin" / "binaries";

            CopyDirectoryRecursively(SourceDirectory  / "NSwag.Console" / "bin" / Configuration / "net461", npmBinariesDirectory / "Win");

            var consoleX86Directory = SourceDirectory / "NSwag.Console.x86" / "bin" / Configuration / "net461";
            CopyFileToDirectory(consoleX86Directory  / "NSwag.x86.exe", npmBinariesDirectory / "Win");
            CopyFileToDirectory(consoleX86Directory  / "NSwag.x86.exe.config", npmBinariesDirectory / "Win");

            Info("Publish .NET Core command line done in prebuild event for NSwagStudio.Installer.wixproj");

            var consoleCoreDirectory = SourceDirectory / "NSwag.ConsoleCore" / "bin" / Configuration;

            CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp2.1/publish", npmBinariesDirectory / "NetCore21");
            // CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp2.2/publish", npmBinariesDirectory / "NetCore22");
            // CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp3.0/publish", npmBinariesDirectory / "NetCore30");
            CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp3.1/publish", npmBinariesDirectory / "NetCore31");

            Info("Package nuspecs");

            NuGetPack(x => x
                .SetTargetPath(SourceDirectory / "NSwag.MSBuild" / "NSwag.MSBuild.nuspec")
            );

            NuGetPack(x => x
                .SetTargetPath(SourceDirectory / "NSwag.ApiDescription.Client" / "NSwag.ApiDescription.Client.nuspec")
            );

            NuGetPack(x => x
                .SetTargetPath(SourceDirectory / "NSwagStudio.Chocolatey" / "NSwagStudio.nuspec")
            );
        });


    // logic from 02_RunUnitTests.bat
    Target UnitTest => _ => _
        .After(Compile)
        .Executes(() =>
        {
            var logger = "";
            if (AppVeyor.Instance is not null)
            {
                logger = "Appveyor";
            }

            VSTest(x => x
                .SetLogger(logger)
                .SetTestAssemblies(SourceDirectory / "NSwag.Generation.WebApi.Tests" / "bin" / Configuration / "NSwag.Generation.WebApi.Tests.dll")
            );

            /*
            VSTest(x => x
                .SetLogger(logger)
                .SetTestAssemblies(SourceDirectory / "NSwag.Tests" / "bin" / Configuration / "NSwag.Tests.dll")
            );
            */

            // project name + target framework pairs
            var dotNetTestTargets = new[]
            {
                ("NSwag.CodeGeneration.Tests", null),
                ("NSwag.CodeGeneration.CSharp.Tests", null),
                ("NSwag.CodeGeneration.TypeScript.Tests", null),
                ("NSwag.Generation.AspNetCore.Tests", null),
                ("NSwag.Core.Tests", null),
                ("NSwag.Core.Yaml.Tests", null),
                ("NSwag.AssemblyLoader.Tests", "netcoreapp2.1")
            };

            foreach (var (project, targetFramework) in dotNetTestTargets)
            {
                DotNetTest(x => x
                    .EnableNoBuild()
                    .EnableNoRestore()
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
            var nswagCommand = SourceDirectory / "NSwagStudio" / "bin" / Configuration / "nswag.cmd";

            // project name + runtime pairs
            var dotnetTargets = new[]
            {
                ("NSwag.Sample.NETCore21", "NetCore21"),
                ("NSwag.Sample.NETCore31", "NetCore31"),
            };

            foreach (var (projectName, runtime) in dotnetTargets)
            {
                var project = Solution.GetProject(projectName);
                DotNetBuild(x => x
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
                );
                var process = StartProcess(nswagCommand, $"run /runtime:{runtime}", workingDirectory: project.Directory);
                process.WaitForExit();
            }
        });

    Target Test => _ => _
        .DependsOn(UnitTest, IntegrationTest);

    // logic from 04_Publish.bat
    Target Publish => _ => _
        .After(Compile)
        .Executes(() =>
        {
            Npm("publish", SourceDirectory / "NSwag.Npm");
        });

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
                string configuration,
                bool build)
            {
                var nswagConfigurationFile = project.Directory / $"{configurationFile}.nswag";
                var nswagSwaggerFile = project.Directory / $"{configurationFile}_swagger.json";

                DeleteFile(nswagSwaggerFile);

                if (build)
                {
                    DotNetBuild(x => x
                        .SetProjectFile(project)
                        .SetConfiguration(configuration)
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
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_assembly", "NetCore21", "Release", true);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_project", "NetCore21", "Release", false);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_reflection", "NetCore21", "Release", true);

            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_assembly", "NetCore21","Debug", true);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_project", "NetCore21", "Debug", false);
            NSwagRun(sampleSolution.GetProject("Sample.AspNetCore21"), "nswag_reflection", "NetCore21", "Debug", true);
        });

}
