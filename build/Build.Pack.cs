using System;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Definition;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Logger;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using Project = Microsoft.Build.Evaluation.Project;

public partial class Build
{
    // logic from 01_Build.bat
    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test, IntegrationTest, UnitTest, Samples)
        .Produces(ArtifactsDirectory / "*.*")
        .Executes(() =>
        {
            if (Configuration != Configuration.Release)
            {
                throw new InvalidOperationException("Cannot pack if compilation hasn't been done in Release mode, use --configuration Release");
            }

            var nugetVersion = VersionPrefix;
            if (!string.IsNullOrWhiteSpace(VersionSuffix))
            {
                nugetVersion += "-" + VersionSuffix;
            }

            EnsureCleanDirectory(ArtifactsDirectory);

            // it seems to cause some headache with publishing, so let's dotnet pack only files we know are suitable
            var projects = SourceDirectory.GlobFiles("**/*.csproj")
                .Where(x => !x.ToString().Contains("_build") &&
                            !x.ToString().Contains("NSwag.Console.csproj") && // legacy .net tool is not published as nuget package
                            !x.ToString().Contains("NSwagStudio") &&
                            !x.ToString().Contains("Test") &&
                            !x.ToString().Contains("Demo") &&
                            !x.ToString().Contains("Integration") &&
                            !x.ToString().Contains("x86") &&
                            !x.ToString().Contains("Launcher") &&
                            !x.ToString().Contains("Sample"))
                .Select(x => Project.FromFile(x, new ProjectOptions()));

            foreach (var project in projects)
            {
                DotNetPack(s => s
                    .SetProcessWorkingDirectory(SourceDirectory)
                    .SetProject(project.FullPath)
                    .SetAssemblyVersion(VersionPrefix)
                    .SetFileVersion(VersionPrefix)
                    .SetInformationalVersion(VersionPrefix)
                    .SetVersion(nugetVersion)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetDeterministic(IsServerBuild)
                    .SetContinuousIntegrationBuild(IsServerBuild)
                );
            }

            var npmBinariesDirectory = SourceDirectory / "NSwag.Npm" / "bin" / "binaries";

            CopyDirectoryRecursively(SourceDirectory / "NSwag.Console" / "bin" / Configuration / "net461", npmBinariesDirectory / "Win");

            var consoleX86Directory = SourceDirectory / "NSwag.Console.x86" / "bin" / Configuration / "net461";

            CopyFileToDirectory(consoleX86Directory / "NSwag.x86.exe", npmBinariesDirectory / "Win");
            CopyFileToDirectory(consoleX86Directory / "NSwag.x86.exe.config", npmBinariesDirectory / "Win");

            Info("Publish .NET Core command line done in prebuild event for NSwagStudio.Installer.wixproj");

            var consoleCoreDirectory = SourceDirectory / "NSwag.ConsoleCore" / "bin" / Configuration;

            CopyDirectoryRecursively(consoleCoreDirectory / "netcoreapp2.1/publish", npmBinariesDirectory / "NetCore21");
            CopyDirectoryRecursively(consoleCoreDirectory / "netcoreapp3.1/publish", npmBinariesDirectory / "NetCore31");
            CopyDirectoryRecursively(consoleCoreDirectory / "net5.0/publish", npmBinariesDirectory / "Net50");
            CopyDirectoryRecursively(consoleCoreDirectory / "net6.0/publish", npmBinariesDirectory / "Net60");

            // gather relevant artifacts
            Info("Package nuspecs");

            var nuspecs = new[]
            {
                SourceDirectory / "NSwag.MSBuild" / "NSwag.MSBuild.nuspec",
                SourceDirectory / "NSwag.ApiDescription.Client" / "NSwag.ApiDescription.Client.nuspec",
                SourceDirectory / "NSwagStudio.Chocolatey" / "NSwagStudio.nuspec"
            };

            foreach (var nuspec in nuspecs)
            {
                NuGetPack(x => x
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetConfiguration(Configuration)
                    .SetVersion(nugetVersion)
                    .SetTargetPath(nuspec)
                );
            }

            var artifacts = Array.Empty<AbsolutePath>()
                .Concat(RootDirectory.GlobFiles("**/Release/**/NSwag*.nupkg"))
                .Concat(SourceDirectory.GlobFiles("**/Release/**/NSwagStudio.msi"));

            foreach (var artifact in artifacts)
            {
                CopyFileToDirectory(artifact, ArtifactsDirectory);
            }

            // patch npm version
            var npmPackagesFile = SourceDirectory / "NSwag.Npm" / "package.json";
            var content = TextTasks.ReadAllText(npmPackagesFile);
            content = Regex.Replace(content, @"""version"": "".*""", @"""version"": """ + VersionPrefix + @"""");
            TextTasks.WriteAllText(npmPackagesFile, content);

            // ZIP directories
            ZipFile.CreateFromDirectory(NSwagNpmBinaries, ArtifactsDirectory / "NSwag.Npm.zip");
            ZipFile.CreateFromDirectory(NSwagStudioBinaries, ArtifactsDirectory / "NSwag.zip");
        });
}

