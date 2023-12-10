using System.Collections.Generic;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

[CustomGitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = new[] { "master", "main" },
    OnPullRequestIncludePaths = new[] { "**/*.*" },
    OnPullRequestExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Compile), nameof(Test), nameof(Pack) },
    CacheKeyFiles = new string[0],
    JobConcurrencyCancelInProgress = true),
]
[CustomGitHubActions(
    "build",
    GitHubActionsImage.WindowsLatest,
    // GitHubActionsImage.UbuntuLatest,
    // GitHubActionsImage.MacOsLatest,
    OnPushBranches = new[] { "master", "main" },
    OnPushTags = new[] { "v*.*.*" },
    OnPushIncludePaths = new[] { "**/*.*" },
    OnPushExcludePaths = new[] { "**/*.md" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Compile), nameof(Test), nameof(Pack), nameof(Publish) },
    ImportSecrets = new[] { "NUGET_API_KEY", "MYGET_API_KEY", "CHOCO_API_KEY", "NPM_AUTH_TOKEN" },
    CacheKeyFiles = new string[0])
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

        // only need to list the ones that are missing from default image
        /*
        newSteps.Insert(0, new GitHubActionsSetupDotNetStep(new[] 
        {
            "8.0.100"
        }));
        */

        newSteps.Insert(0, new GitHubActionsUseGnuTarStep());
        newSteps.Insert(0, new GitHubActionsConfigureLongPathsStep());

        // add artifacts manually as they would otherwise by hard to configure via attributes
        if (PublishArtifacts)
        {
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwag.zip", Path = "artifacts/NSwag.zip" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwag.Npm.zip", Path = "artifacts/NSwag.Npm.zip" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwagStudio.msi", Path = "artifacts/NSwagStudio.msi" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NuGet Packages", Path = "artifacts/*.nupkg" });
        }

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
    public GitHubActionsSetupDotNetStep(string[] versions)
    {
        Versions = versions;
    }

    string[] Versions { get; }

    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- uses: actions/setup-dotnet@v3");

        using (writer.Indent())
        {
            writer.WriteLine("with:");
            using (writer.Indent())
            {
                writer.WriteLine("dotnet-version: |");
                using (writer.Indent())
                {
                    foreach (var version in Versions)
                    {
                        writer.WriteLine(version);
                    }
                }
            }
        }
    }
}

class GitHubActionsUseGnuTarStep : GitHubActionsStep
{
    public override void Write(CustomFileWriter writer)
    {
        writer.WriteLine("- if: ${{ runner.os == 'Windows' }}");
        using (writer.Indent())
        {
            writer.WriteLine("name: 'Use GNU tar'");
            writer.WriteLine("shell: cmd");
            writer.WriteLine("run: |");
            using (writer.Indent())
            {
                writer.WriteLine("echo \"Adding GNU tar to PATH\"");
                writer.WriteLine("echo C:\\Program Files\\Git\\usr\\bin>>\"%GITHUB_PATH%\"");
            }
        }
    }
}