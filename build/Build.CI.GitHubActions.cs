using System.Collections.Generic;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

[CustomGitHubActionsAttribute(
    "pr",
    GitHubActionsImage.WindowsServer2022,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = new[] { "master", "main" },
    OnPullRequestIncludePaths = new[] { "**/*.*" },
    OnPullRequestExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(InstallDependencies), nameof(Compile), nameof(Test), nameof(Pack) },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "src/**/package.json" }),
]
[CustomGitHubActionsAttribute(
    "build",
    GitHubActionsImage.WindowsServer2022,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPushBranches = new[] { "master", "main" },
    OnPushTags = new[] { "v*.*.*" },
    OnPushIncludePaths = new[] { "**/*.*" },
    OnPushExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(InstallDependencies), nameof(Compile), nameof(Test), nameof(Pack), nameof(Publish) },
    ImportSecrets = new[] { "NUGET_API_KEY", "MYGET_API_KEY", "CHOCO_API_KEY", "NPM_AUTH_TOKEN" },
    CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "src/**/package.json" })
]
public partial class Build
{
}

class CustomGitHubActionsAttribute : GitHubActionsAttribute
{
    public CustomGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images) : base(name, image, images)
    {
    }

    protected override GitHubActionsJob GetJobs(GitHubActionsImage image, IReadOnlyCollection<ExecutableTarget> relevantTargets)
    {
        var job = base.GetJobs(image, relevantTargets);

        var newSteps = new List<GitHubActionsStep>(job.Steps);
        foreach (var version in new[] { "7.0.*", "6.0.*", "5.0.*", "3.1.*", "2.1.*" })
        {
            newSteps.Insert(1, new GitHubActionsSetupDotNetStep
            {
                Version = version
            });
        }
        
        newSteps.Insert(0, new GitHubActionsConfigureLongPathsStep());

        job.Steps = newSteps.ToArray();
        return job;
    }
}

class GitHubActionsConfigureLongPathsStep : GitHubActionsStep
{
    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- name: 'Allow long file path'");
        using (writer.Indent())
        {
            writer.WriteLine("run: git config --system core.longpaths true");
        }
    }
}

class GitHubActionsSetupDotNetStep : GitHubActionsStep
{
    public string Version { get; init; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- uses: actions/setup-dotnet@v3");

        using (writer.Indent())
        {
            writer.WriteLine("with:");
            using (writer.Indent())
            {
                writer.WriteLine($"dotnet-version: {Version}");
            }
        }
    }
}
