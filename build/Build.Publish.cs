using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Chocolatey;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;

public partial class Build
{
    string NuGetSource => "https://api.nuget.org/v3/index.json";
    [Parameter] [Secret] string NuGetApiKey;

    string MyGetGetSource => "https://www.myget.org/F/nswag/api/v2/package";
    [Parameter] [Secret] string MyGetApiKey;

    [Parameter] [Secret] string ChocoApiKey;

    string ApiKeyToUse => IsTaggedBuild ? NuGetApiKey : MyGetApiKey;
    string SourceToUse => IsTaggedBuild ? NuGetSource : MyGetGetSource;

    Target Publish => _ => _
        .OnlyWhenDynamic(() => IsRunningOnWindows && (GitRepository.IsOnMainOrMasterBranch() || IsTaggedBuild) && GitRepository.GetGitHubOwner() == "RicoSuter")
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey, () => MyGetApiKey, () => ChocoApiKey)
        .Executes(() =>
        {
            // Each registry is pushed independently and the failures are collected. These run in
            // one method, so letting the first one throw decides what the rest of the release
            // does: an npm credential problem would abort before the NuGet push and ship
            // Chocolatey alone, with no way to re-run it cleanly.
            var failures = new List<Exception>();

            void Push(string registry, Action push)
            {
                try
                {
                    push();
                }
                catch (Exception e)
                {
                    Serilog.Log.Error("Push to {Registry} failed: {Message}", registry, e.Message);
                    failures.Add(new InvalidOperationException($"Push to {registry} failed: {e.Message}", e));
                }
            }

            if (IsTaggedBuild)
            {
                Push("Chocolatey", () => ChocolateyPush(_ => _
                    .SetApiKey(ChocoApiKey)
                    .SetPathToNuGetPackage(ArtifactsDirectory.GlobFiles("NSwagStudio.*.nupkg").Single())
                    .SetSource("https://push.chocolatey.org/")
                ));

                // npm trusted publishing: the CLI exchanges the workflow's OIDC token for a
                // short-lived credential, so there is no token to store or rotate. It needs a
                // trusted publisher on npmjs.com for this repository and build.yml, id-token:
                // write in the workflow, and npm 11.5.1 or later, which the workflow installs.
                //
                // do not publish preview packages to npm
                if (string.IsNullOrEmpty(VersionSuffix))
                {
                    Push("npm", () =>
                    {
                        var outputs = Npm("publish", SourceDirectory / "NSwag.Npm", logOutput: false);

                        foreach (var output in outputs.Where(o => !o.Text.Contains("npm notice")))
                        {
                            if (output.Type == OutputType.Std)
                            {
                                Serilog.Log.Information(output.Text);
                            }
                            else
                            {
                                Serilog.Log.Error(output.Text);
                            }
                        }
                    });
                }
            }

            Push(IsTaggedBuild ? "NuGet" : "MyGet", () => DotNetNuGetPush(_ => _
                    .Apply(PushSettingsBase)
                    .Apply(PushSettings)
                    .CombineWith(PushPackageFiles, (_, v) => _
                        .SetTargetPath(v))
                    .Apply(PackagePushSettings),
                PushDegreeOfParallelism,
                PushCompleteOnFailure));

            if (failures.Count == 0)
            {
                return;
            }

            if (IsTaggedBuild)
            {
                // A tagged build that could not reach a registry is a broken release. Everything
                // that did succeed has still been pushed, so the gap is named rather than hidden.
                throw new AggregateException(
                    "One or more pushes failed on a tagged build; see the errors above for which.",
                    failures);
            }
        });

    Configure<DotNetNuGetPushSettings> PushSettingsBase => _ => _
        .SetSource(SourceToUse)
        .SetApiKey(ApiKeyToUse)
        .SetSkipDuplicate(true);

    Configure<DotNetNuGetPushSettings> PushSettings => _ => _;
    Configure<DotNetNuGetPushSettings> PackagePushSettings => _ => _;

    IEnumerable<AbsolutePath> PushPackageFiles =>
        ArtifactsDirectory.GlobFiles("*.nupkg")
            .Where(x => !x.ToString().Contains("NSwagStudio", StringComparison.OrdinalIgnoreCase));

    bool PushCompleteOnFailure => true;

    int PushDegreeOfParallelism => 1;
}