using System.Runtime.CompilerServices;

namespace NSwag.CodeGeneration.Tests;

public static class VerifyHelper
{
    /// <summary>
    /// Helper to run verify tests with sane defaults.
    /// </summary>
    public static SettingsTask Verify(string output, bool scrubPragmas = true, bool scrubApiComments = true, [CallerFilePath] string sourceFile = "")
    {
        var settingsTask = Verifier
            .Verify(output, sourceFile: sourceFile);

        settingsTask = settingsTask
            .ScrubLinesContaining(
                StringComparison.OrdinalIgnoreCase,
                "Generated using the NSwag toolchain",
                "Generated using the NJsonSchema",
                "x-generator",
                "[System.CodeDom.Compiler.GeneratedCode(\"NSwag\",",
                "[System.CodeDom.Compiler.GeneratedCode(\"NJsonSchema\"",
                "auto-generated>",
                "//-------------");

        if (scrubPragmas)
        {
            settingsTask = settingsTask.ScrubLines(x => x.StartsWith("#pragma", StringComparison.Ordinal));
        }

        if (scrubApiComments)
        {
            settingsTask = settingsTask.ScrubLines(x =>
            {
                var trimmed = x.TrimStart();
                return trimmed.StartsWith("/// ", StringComparison.Ordinal)
                       || trimmed.StartsWith("/* ", StringComparison.Ordinal)
                       || trimmed.StartsWith("* ", StringComparison.Ordinal);
            });
        }

        return settingsTask
            .UseDirectory("Snapshots")
            .AutoVerify(includeBuildServer: false);
    }
}