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

using static Nuke.Common.Tools.Chocolatey.ChocolateyTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;
using static Nuke.Common.Logger;

public partial class Build
{
    string NuGetSource => "https://api.nuget.org/v3/index.json";
    [Parameter] [Secret] string NuGetApiKey;

    string MyGetGetSource => "https://www.myget.org/F/nswag/api/v2/package";
    [Parameter] [Secret] string MyGetApiKey;

    [Parameter] [Secret] string ChocoApiKey;

    [Parameter] [Secret] string NpmAuthToken;

    string ApiKeyToUse => !string.IsNullOrWhiteSpace(TagVersion) ? NuGetApiKey : MyGetApiKey;
    string SourceToUse => !string.IsNullOrWhiteSpace(TagVersion) ? NuGetSource : MyGetGetSource;

    Target Publish => _ => _
        // TODO remove nuke-publish
        .OnlyWhenDynamic(() => IsRunningOnWindows && (GitRepository.IsOnMainOrMasterBranch() || GitRepository.Branch == "nuke-publish"))
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey, () => MyGetApiKey, () => ChocoApiKey, () => NpmAuthToken)
        .Executes(() =>
        {
            // this is a bit problematic, we can now fail in only tagging condition as this publish is a bit black-box
            if (!string.IsNullOrWhiteSpace(TagVersion))
            {
                try
                {
                    var userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    File.WriteAllText(
                        Path.Combine(userDirectory, ".npmrc"),
                        "//registry.npmjs.org/:_authToken=" + NpmAuthToken + "\n");

                    var outputs = Npm("publish", SourceDirectory / "NSwag.Npm", logOutput: false);

                    foreach (var output in outputs.Where(o => !o.Text.Contains("npm notice")))
                    {
                        if (output.Type == OutputType.Std)
                        {
                            Info(output.Text);
                        }
                        else
                        {
                            Error(output.Text);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("NPM PUBLISH FAILED: " + ex.Message);
                }
            }

            DotNetNuGetPush(_ => _
                    .Apply(PushSettingsBase)
                    .Apply(PushSettings)
                    .CombineWith(PushPackageFiles, (_, v) => _
                        .SetTargetPath(v))
                    .Apply(PackagePushSettings),
                PushDegreeOfParallelism,
                PushCompleteOnFailure);

            if (!string.IsNullOrWhiteSpace(TagVersion))
            {
                ChocolateyPush(_ => _
                    .SetApiKey(ChocoApiKey)
                    .SetPathToNuGetPackage(ArtifactsDirectory.GlobFiles("NSwagStudio.*.nupkg").Single())
                );
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

    int PushDegreeOfParallelism => 2;
}