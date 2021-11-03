using Nuke.Common.CI.GitHubActions;

[GitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = new [] { "master" },
    // OnPushBranchesIgnore = new[] { MasterBranch, ReleaseBranchPrefix + "/*" },
    //OnPullRequestBranches = new[] { DevelopBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(InstallDependencies), nameof(Compile), nameof(Test), nameof(Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "src/**/package.json" })
]
[GitHubActions(
    "build",
    GitHubActionsImage.WindowsLatest,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPushBranches = new [] { "master" },
    // OnPushBranchesIgnore = new[] { MasterBranch, ReleaseBranchPrefix + "/*" },
    //OnPullRequestBranches = new[] { DevelopBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(InstallDependencies), nameof(Compile), nameof(Test), nameof(Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "src/**/package.json" })
]
public partial class Build
{
}
