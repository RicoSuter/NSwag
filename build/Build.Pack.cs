using System;
using System.IO.Compression;
using System.Linq;
using Microsoft.Build.Definition;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
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
        .Produces(ArtifactsDirectory / "*.*")
        .Executes(() =>
        {
            if (Configuration != Configuration.Release)
            {
                throw new InvalidOperationException("Cannot pack if compilation hasn't been done in Release mode, use --configuration Release");
            }

            EnsureCleanDirectory(ArtifactsDirectory);

            // it seems to cause some headache with publishing, so let's dotnet pack only files we know are suitable
            var projects = SourceDirectory.GlobFiles("**/*.csproj")
                .Where(x => x.ToString().Contains("NSwagStudio"))
                .Select(x => Project.FromFile(x, new ProjectOptions()))
                .Where(x => !string.Equals(x.GetProperty("IsPackable")?.EvaluatedValue, "true", StringComparison.OrdinalIgnoreCase));

            foreach (var project in projects)
            {
                DotNetPack(s => s
                    .SetProcessWorkingDirectory(SourceDirectory)
                    .SetProject(project.FullPath)
                    .SetAssemblyVersion(TagVersion)
                    .SetFileVersion(TagVersion)
                    .SetInformationalVersion(TagVersion)
                    .SetVersionSuffix(VersionSuffix)
                    .SetConfiguration(Configuration)
                    .SetOutputDirectory(ArtifactsDirectory)
                );
            }

            var npmBinariesDirectory = SourceDirectory / "NSwag.Npm" / "bin" / "binaries";

            CopyDirectoryRecursively(SourceDirectory  / "NSwag.Console" / "bin" / Configuration / "net461", npmBinariesDirectory / "Win");

            var consoleX86Directory = SourceDirectory / "NSwag.Console.x86" / "bin" / Configuration / "net461";
            CopyFileToDirectory(consoleX86Directory  / "NSwag.x86.exe", npmBinariesDirectory / "Win");
            CopyFileToDirectory(consoleX86Directory  / "NSwag.x86.exe.config", npmBinariesDirectory / "Win");

            Info("Publish .NET Core command line done in prebuild event for NSwagStudio.Installer.wixproj");

            var consoleCoreDirectory = SourceDirectory / "NSwag.ConsoleCore" / "bin" / Configuration;

            CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp2.1/publish", npmBinariesDirectory / "NetCore21");
            CopyDirectoryRecursively(consoleCoreDirectory  / "netcoreapp3.1/publish", npmBinariesDirectory / "NetCore31");
            CopyDirectoryRecursively(consoleCoreDirectory  / "net5.0/publish", npmBinariesDirectory / "Net50");
            CopyDirectoryRecursively(consoleCoreDirectory  / "net6.0/publish", npmBinariesDirectory / "Net60");

            // gather relevant artifacts
            Info("Package nuspecs");

            NuGetPack(x => x
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration)
                .SetTargetPath(SourceDirectory / "NSwag.MSBuild" / "NSwag.MSBuild.nuspec")
            );

            NuGetPack(x => x
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration)
                .SetTargetPath(SourceDirectory / "NSwag.ApiDescription.Client" / "NSwag.ApiDescription.Client.nuspec")
            );

            NuGetPack(x => x
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration)
                .SetTargetPath(SourceDirectory / "NSwagStudio.Chocolatey" / "NSwagStudio.nuspec")
            );

            var artifacts = Array.Empty<AbsolutePath>()
                .Concat(RootDirectory.GlobFiles("**/Release/**/NSwag*.nupkg"))
                .Concat(SourceDirectory.GlobFiles("**/Release/**/NSwagStudio.msi"))
                .Concat(SourceDirectory.GlobFiles("**/NSwagStudio/Properties/AssemblyInfo.cs"));

            foreach (var artifact in artifacts)
            {
                CopyFileToDirectory(artifact, ArtifactsDirectory);
            }

            // ZIP directories
            ZipFile.CreateFromDirectory(NSwagNpmBinaries, ArtifactsDirectory / "NSwag.Npm.zip");
            ZipFile.CreateFromDirectory(NSwagStudioBinaries, ArtifactsDirectory / "NSwag.zip");
        });
}

