using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NSwag.CodeGeneration.TypeScript.Tests;

/// <summary>
/// Compiles a generated TypeScript client together with a small harness and executes it under node,
/// returning everything the harness printed to stdout. Used to assert the *runtime* value a client
/// resolves to (as opposed to <see cref="TypeScriptCompiler"/>, which only type-checks).
/// </summary>
public class TypeScriptRunner
{
    private static readonly Lazy<string> NpxPath = new(() => FindExecutable("npx"));
    private static readonly Lazy<string> NodePath = new(() => FindExecutable("node"));

    /// <summary>
    /// Writes <paramref name="source"/> to a temp file in the test project (so node resolves the
    /// project's node_modules), compiles it to CommonJS JavaScript with tsc, runs the emitted file with
    /// node and returns its stdout. Fails the test if compilation or execution does not exit cleanly.
    /// </summary>
    public static string Run(string source)
    {
        // The temp file must sit in the source test project directory, next to package.json / tsconfig.json
        // and node_modules, so tsc and node resolve the project's dependencies (typescript, axios). We locate
        // that directory from this source file's compile-time path rather than counting "../" hops from the
        // (runner-dependent) current directory or output layout.
        var workingDirectory = ProjectDirectory();

        var id = Guid.NewGuid().ToString("N");
        var tsFilePath = Path.Combine(workingDirectory, $"runtime_{id}.ts");
        var jsFilePath = tsFilePath.Replace(".ts", ".js");
        File.WriteAllText(tsFilePath, source);

        try
        {
            // Compile to runnable CommonJS. Passing the file on the command line makes tsc ignore the
            // project tsconfig, so we set the options we rely on explicitly (DOM lib for fetch/Response,
            // es2017 so async/await runs directly under node, node module resolution for the axios import).
            var (compileExit, compileOut, compileErr) = Exec(
                NpxPath.Value,
                $"tsc --module commonjs --target es2017 --lib es2017,dom --moduleResolution node \"{tsFilePath}\"",
                workingDirectory);

            Assert.True(compileExit == 0, $"TypeScript compilation failed:\n{compileOut}\n{compileErr}\n\n{source}\n\n");

            var (runExit, runOut, runErr) = Exec(NodePath.Value, $"\"{jsFilePath}\"", workingDirectory);
            Assert.True(runExit == 0, $"Node execution failed:\n{runOut}\n{runErr}\n\n{source}\n\n");

            return runOut;
        }
        finally
        {
            if (File.Exists(tsFilePath))
            {
                File.Delete(tsFilePath);
            }

            if (File.Exists(jsFilePath))
            {
                File.Delete(jsFilePath);
            }
        }
    }

    /// <summary>
    /// Returns the directory of this source file, which is the test project root (where package.json,
    /// tsconfig.json and node_modules live). Uses <see cref="CallerFilePathAttribute"/> so it does not
    /// depend on the current working directory or the build output layout.
    /// </summary>
    private static string ProjectDirectory([CallerFilePath] string sourceFilePath = "")
    {
        return Path.GetDirectoryName(sourceFilePath)!;
    }

    private static (int ExitCode, string StdOut, string StdErr) Exec(string fileName, string arguments, string workingDirectory)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var stdOut = process.StandardOutput.ReadToEnd();
        var stdErr = process.StandardError.ReadToEnd();
        process.WaitForExit(30_000);

        return (process.ExitCode, stdOut, stdErr);
    }

    private static string FindExecutable(string name)
    {
        var locator = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = locator,
                    Arguments = name,
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
                   ?? throw new InvalidOperationException($"Could not find {name} executable");
        }
        catch
        {
            return null;
        }
    }
}
