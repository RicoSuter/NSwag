using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NSwag.CodeGeneration.TypeScript.Tests;

public class TypeScriptCompiler
{
    private static readonly Lazy<string> NpxPath = new(FindNpxExecutable);

    public static void AssertCompile(string source)
    {
        var workingDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "../../../../src/NSwag.CodeGeneration.TypeScript.Tests");

        var tempFilePath = Path.Combine(workingDirectory, $"temp_{Guid.NewGuid()}.ts");
        File.WriteAllText(tempFilePath, source);

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = workingDirectory,
                    FileName = NpxPath.Value,
                    Arguments = $"tsc \"{tempFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var stdOutput = process.StandardOutput.ReadToEnd();
            var stdError = process.StandardError.ReadToEnd();
            process.WaitForExit(10_000);

            Assert.True(process.ExitCode == 0, $"TypeScript compilation failed:\n{stdOutput}\n{stdError}\n\n{source}\n\n");
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }

            var jsFilePAth = tempFilePath!.Replace(".ts", ".js");
            if (File.Exists(jsFilePAth))
            {
                File.Delete(jsFilePAth);
            }
        }
    }


    private static string FindNpxExecutable()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? FindNpxExecutable("where")
            : FindNpxExecutable("which");
    }

    private static string FindNpxExecutable(string locator)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = locator,
                    Arguments = "npx",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
            return lines.FirstOrDefault(x => x.EndsWith(".cmd", StringComparison.Ordinal))
                   ?? lines.FirstOrDefault()
                   ?? throw new InvalidOperationException("Could not find tsc executable");
        }
        catch
        {
            return null;
        }
    }
}