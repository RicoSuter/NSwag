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

    /// <summary>
    /// Constructs a namespace-like identifier based on the caller's file name (without extension) and the caller member name.
    /// </summary>
    /// <param name="sourceMethod">
    /// The caller member name populated by the compiler via <see cref="CallerMemberNameAttribute"/>.
    /// </param>
    /// <param name="sourceFile">
    /// The caller file path populated by the compiler via <see cref="CallerFilePathAttribute"/>.
    /// </param>
    /// <returns>
    /// A string in the format "<c>{FileNameWithoutExtension}.{MemberName}</c>" (e.g. <c>MyTests.TestMethod</c>).
    /// </returns>
    public static string GetNameSpace([CallerMemberName] string sourceMethod = "", [CallerFilePath] string sourceFile = "")
    {
        return $"{Path.GetFileNameWithoutExtension(sourceFile)}.{sourceMethod}";
    }
}