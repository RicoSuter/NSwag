using System;
using System.Collections.Generic;

using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;

public partial class Build
{
    string NuGetSource => "https://api.nuget.org/v3/index.json";
    [Parameter] [Secret] string NuGetApiKey;

    string MyGetGetSource => "https://www.myget.org/F/nswag/api/v2/package";
    [Parameter] [Secret] string MyGetApiKey;

    string ApiKeyToUse => !string.IsNullOrWhiteSpace(TagVersion) ? NuGetApiKey : MyGetApiKey;
    string SourceToUse => !string.IsNullOrWhiteSpace(TagVersion) ? NuGetSource : MyGetGetSource;

    Target Publish => _ => _
         // TODO remove nuke-publish
        .OnlyWhenDynamic(() => IsRunningOnWindows && (GitRepository.IsOnMainOrMasterBranch() || GitRepository.Branch == "nuke-publish"))
        .DependsOn(Pack)
        .Requires(() => NuGetApiKey, () => MyGetApiKey)
        .Executes(() =>
        {
            // this is a bit problematic, we can now fail in only tagging condition as this publish is a bit black-box
            if (string.IsNullOrWhiteSpace(TagVersion))
            {
                try
                {
                    Npm("publish", SourceDirectory / "NSwag.Npm");
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
        });

    Configure<DotNetNuGetPushSettings> PushSettingsBase => _ => _
        .SetSource(SourceToUse)
        .SetApiKey(ApiKeyToUse)
        .SetSkipDuplicate(true);

    Configure<DotNetNuGetPushSettings> PushSettings => _ => _;
    Configure<DotNetNuGetPushSettings> PackagePushSettings => _ => _;

    IEnumerable<AbsolutePath> PushPackageFiles => ArtifactsDirectory.GlobFiles("*.nupkg");

    bool PushCompleteOnFailure => true;

    int PushDegreeOfParallelism => 2;
}