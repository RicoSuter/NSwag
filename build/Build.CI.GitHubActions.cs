using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Execution;
using Nuke.Common.Utilities;

[CustomGitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = ["master", "main"],
    OnPullRequestIncludePaths = ["**/*.*"],
    OnPullRequestExcludePaths = ["**/*.md"],
    PublishArtifacts = true,
    InvokedTargets = [nameof(Compile), nameof(Test), nameof(Pack)],
    CacheKeyFiles = [],
    ConcurrencyCancelInProgress = true),
]
[CustomGitHubActions(
    "build",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPushBranches = ["master", "main"],
    OnPushTags = ["v*.*.*"],
    OnPushIncludePaths = ["**/*.*"],
    OnPushExcludePaths = ["**/*.md"],
    PublishArtifacts = true,
    InvokedTargets = [nameof(Compile), nameof(Test), nameof(Pack), nameof(Publish)],
    // NUGET_API_KEY is not imported as a secret: trusted publishing supplies it at
    // run time from the NuGet/login step (see CustomGitHubActionsAttribute.GetJobs).
    ImportSecrets = ["MYGET_API_KEY", "CHOCO_API_KEY", "NPM_AUTH_TOKEN"],
    // id-token lets GitHub mint the OIDC token that NuGet/login exchanges for a
    // short-lived API key. NUKE writes permissions at workflow level and emits only
    // what is listed here, and GitHub sets every unlisted scope to none, so
    // contents: read must be declared for checkout and caching to keep working.
    WritePermissions = [GitHubActionsPermissions.IdToken],
    ReadPermissions = [GitHubActionsPermissions.Contents],
    CacheKeyFiles = [])
]
public partial class Build;

class CustomGitHubActionsAttribute : GitHubActionsAttribute
{
    public CustomGitHubActionsAttribute(string name, GitHubActionsImage image, params GitHubActionsImage[] images) : base(name, image, images)
    {
    }

    protected override GitHubActionsJob GetJobs(GitHubActionsImage image, IReadOnlyCollection<ExecutableTarget> relevantTargets)
    {
        var job = base.GetJobs(image, relevantTargets);

        var newSteps = new List<GitHubActionsStep>(job.Steps);

        newSteps.Insert(0, new GitHubActionsSetupDotNetStep(["10.0"]));

        var onWindows = image.ToString().StartsWith("windows", StringComparison.OrdinalIgnoreCase);
        if (onWindows)
        {
            newSteps.Insert(0, new GitHubActionsUseGnuTarStep());
            newSteps.Insert(0, new GitHubActionsConfigureLongPathsStep());
        }

        // add artifacts manually as they would otherwise by hard to configure via attributes
        if (PublishArtifacts && onWindows)
        {
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwag.zip", Path = "artifacts/NSwag.zip" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwag.Npm.zip", Path = "artifacts/NSwag.Npm.zip" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NSwagStudio.msi", Path = "artifacts/NSwagStudio.msi" });
            newSteps.Add(new GitHubActionsArtifactStep { Name = "NuGet Packages", Path = "artifacts/*.nupkg" });
        }

        // Trusted publishing replaces the stored NUGET_API_KEY secret with a short-lived
        // key fetched from the NuGet/login step.
        //
        // The step is added to every image, not just Windows. Publish only ever pushes on
        // Windows (OnlyWhenDynamic), but NUKE validates .Requires() against the scheduled
        // targets before any dynamic condition is evaluated, so a job whose NUGET_API_KEY
        // resolved to an empty string would fail with "Target 'Publish' requires member
        // 'NuGetApiKey' to be not null or empty" before it ever skips the target.
        //
        // The key is valid for one hour, which comfortably covers Restore/Compile/Test/Pack.
        var runStep = newSteps.OfType<GitHubActionsRunStep>()
            .FirstOrDefault(x => x.InvokedTargets.Contains("Publish"));

        if (runStep != null)
        {
            newSteps.Insert(newSteps.IndexOf(runStep), new GitHubActionsNuGetLoginStep());
            runStep.Imports["NUGET_API_KEY"] = "${{ steps.nuget-login.outputs.NUGET_API_KEY }}";
        }

        job.Steps = newSteps.ToArray();
        return job;
    }
}

class GitHubActionsNuGetLoginStep : GitHubActionsStep
{
    public override void Write(CustomFileWriter writer)
    {
        // Exchange the GitHub OIDC token for a short-lived (1 hour) NuGet API key via trusted publishing.
        // Requires a matching trusted publishing policy on nuget.org (repository, workflow file build.yml).
        writer.WriteLine("- name: 'NuGet login'");
        using (writer.Indent())
        {
            writer.WriteLine("uses: NuGet/login@v1");
            writer.WriteLine("id: nuget-login");
            writer.WriteLine("with:");
            using (writer.Indent())
            {
                // nuget.org profile name, not the email address
                writer.WriteLine("user: ${{ secrets.NUGET_USER }}");
            }
        }
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
        writer.WriteLine("- uses: actions/setup-dotnet@v5");

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