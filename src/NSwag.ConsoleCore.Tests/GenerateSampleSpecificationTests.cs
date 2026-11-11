using System.Diagnostics;
using NSwag.CodeGeneration.CSharp.Tests;
using NSwag.CodeGeneration.TypeScript.Tests;

namespace NSwag.ConsoleCore.Tests
{
    public class GenerateSampleSpecificationTests
    {
        [Theory]
        [InlineData("NSwag.Sample.NET80", "net8.0", true)]
        [InlineData("NSwag.Sample.NET80Minimal", "net8.0", true)]
        [InlineData("NSwag.Sample.NET90", "net9.0", true)]
        [InlineData("NSwag.Sample.NET90Minimal", "net9.0", true)]
        [InlineData("NSwag.Sample.NET100", "net10.0", true)]
        [InlineData("NSwag.Sample.NET100Minimal", "net10.0", true)]
        [InlineData("NSwag.Sample.NET110", "net11.0", true)]
        [InlineData("NSwag.Sample.NET110Minimal", "net11.0", true)]
        public async Task Should_generate_openapi_for_project(string projectName, string targetFramework, bool generatesCode)
        {
            // Arrange
#if DEBUG
            const string configuration = "debug";
#else
            const string configuration = "release";
#endif
            var executablePath = Path.GetFullPath($"../../../../artifacts/bin/NSwag.ConsoleCore/{configuration}_{targetFramework}/dotnet-nswag.dll");
            var nswagJsonPath = Path.GetFullPath($"../../../../src/{projectName}/nswag.json");
            var openApiJsonPath = Path.GetFullPath($"../../../../src/{projectName}/openapi.json");

            var generatedClientsCsPath = Path.GetFullPath($"../../../../src/{projectName}/GeneratedClientsCs.gen");
            var generatedClientsTsPath = Path.GetFullPath($"../../../../src/{projectName}/GeneratedClientsTs.gen");
            var generatedControllersCsPath = Path.GetFullPath($"../../../../src/{projectName}/GeneratedControllersCs.gen");

            File.Delete(openApiJsonPath);
            File.Delete(generatedClientsTsPath);
            File.Delete(generatedClientsCsPath);
            File.Delete(generatedControllersCsPath);
            Assert.False(File.Exists(openApiJsonPath));

            // Act
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = executablePath + " run " + nswagJsonPath,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                Environment =
                {
                    { "NSWAG_NOVERSION", "true" }
                }
            });

            // Read stdout/stderr asynchronously to avoid deadlock when pipe buffers fill up
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            // The first run of a sample restores and builds the project, which is slow on a cold
            // CI runner. The reads are bounded as well as the exit: WaitForExit does not wait for
            // the redirected streams, and MSBuild worker nodes inherit those pipes and can outlive
            // dotnet-nswag, so awaiting them on their own can block for ever.
            var timeout = TimeSpan.FromMinutes(5);
            using var timeoutCts = new CancellationTokenSource(timeout);

            try
            {
                await process.WaitForExitAsync(timeoutCts.Token);
                await Task.WhenAll(outputTask, errorTask).WaitAsync(timeoutCts.Token);
            }
            catch (Exception exception)
            {
                // The timeout arrives as OperationCanceledException, a broken pipe as IOException.
                // Either way the process may still be running, so it goes before the test reports.
                try
                {
                    process.Kill(entireProcessTree: true);
                    await process.WaitForExitAsync();
                }
                catch (InvalidOperationException)
                {
                    // the process is already gone
                }

                string captured;
                try
                {
                    // Killing the tree closes the pipes, so this normally completes at once.
                    captured = string.Concat(
                        await Task.WhenAll(outputTask, errorTask).WaitAsync(TimeSpan.FromSeconds(30)));
                }
                catch (Exception)
                {
                    captured = "(the process output could not be read)";
                }

                var reason = exception is OperationCanceledException
                    ? $"did not finish within {timeout.TotalMinutes:0} minutes"
                    : $"failed with {exception.GetType().Name}: {exception.Message}";

                Assert.Fail($"dotnet-nswag {reason} for {projectName} ({targetFramework})." +
                            Environment.NewLine + captured);
            }

            var output = await outputTask;
            var error = await errorTask;

            // Assert
            if (process.ExitCode != 0)
            {
                Assert.Fail(output + error);
            }

            var json = await File.ReadAllTextAsync(openApiJsonPath);
            await Verifier
                .Verify(json)
                .ScrubLinesContaining("x-generator")
                .UseParameters(projectName, targetFramework, generatesCode)
                .AutoVerify(includeBuildServer: false);

            if (generatesCode)
            {
                await CheckTypeScriptAsync(projectName, targetFramework, generatesCode, generatedClientsTsPath);
                await CheckCSharpClientsAsync(projectName, targetFramework, generatesCode, generatedClientsCsPath);
                await CheckCSharpControllersAsync(projectName, targetFramework, generatesCode, generatedControllersCsPath);
            }
        }

        private static async Task CheckCSharpControllersAsync(string projectName, string targetFramework, bool generatesCode, string generatedControllersCsPath)
        {
            var code = await File.ReadAllTextAsync(generatedControllersCsPath);
            await Verifier.Verify(code)
                .UseMethodName(nameof(CheckCSharpControllersAsync))
                .UseParameters(projectName, targetFramework, generatesCode)
                .AutoVerify(includeBuildServer: false);

            CSharpCompiler.AssertCompile(code);
        }

        private static async Task CheckCSharpClientsAsync(string projectName, string targetFramework, bool generatesCode, string generatedClientsCsPath)
        {
            var code = await File.ReadAllTextAsync(generatedClientsCsPath);
            await Verifier.Verify(code)
                .UseMethodName(nameof(CheckCSharpClientsAsync))
                .UseParameters(projectName, targetFramework, generatesCode)
                .AutoVerify(includeBuildServer: false);

            CSharpCompiler.AssertCompile(code);
        }

        private static async Task CheckTypeScriptAsync(string projectName, string targetFramework, bool generatesCode, string generatedClientsTsPath)
        {
            var code = await File.ReadAllTextAsync(generatedClientsTsPath);
            await Verifier.Verify(code)
                .UseMethodName(nameof(CheckTypeScriptAsync))
                .UseParameters(projectName, targetFramework, generatesCode)
                .AutoVerify(includeBuildServer: false);

            TypeScriptCompiler.AssertCompile(code);
        }
    }
}