using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace NSwag.ConsoleCore.Tests
{
    [UsesVerify]
    public class GenerateSampleSpecificationTests
    {
        [Theory]
        [InlineData("NSwag.Sample.NET60", "net6.0", false)]
        [InlineData("NSwag.Sample.NET60Minimal", "net6.0", false)]
        [InlineData("NSwag.Sample.NET70", "net7.0", false)]
        [InlineData("NSwag.Sample.NET70Minimal", "net7.0", true)]
        public async Task Should_generate_openapi_for_project(string projectName, string targetFramework, bool generatesCode)
        {
            // Arrange
#if DEBUG
            const string configuration = "Debug";
#else
            const string configuration = "Release";
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
                RedirectStandardOutput = true
            });

            try
            {
                process.WaitForExit(20000);
            }
            finally
            {
                process.Kill();
            }

            // Assert
            if (process.ExitCode != 0)
            {
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                Assert.True(false, output + error);
            }

            var json = File.ReadAllText(openApiJsonPath);
            json = Regex.Replace(json, "\"NSwag v.*\"", "\"NSwag\"");
            await Verifier.Verify(json).UseParameters(projectName, targetFramework, generatesCode);

            if (generatesCode)
            {
                await CheckTypeScriptAsync(projectName, targetFramework, generatesCode, generatedClientsTsPath);
                await CheckCSharpClientsAsync(projectName, targetFramework, generatesCode, generatedClientsCsPath);
                await CheckCSharpControllersAsync(projectName, targetFramework, generatesCode, generatedControllersCsPath);
            }
        }

        private static async Task CheckCSharpControllersAsync(string projectName, string targetFramework, bool generatesCode, string generatedControllersCsPath)
        {
            var code = File.ReadAllText(generatedControllersCsPath);
            code = Regex.Replace(code, "NSwag v.*\\)", "NSwag");
            await Verifier.Verify(code).UseMethodName(nameof(CheckCSharpControllersAsync)).UseParameters(projectName, targetFramework, generatesCode);
        }

        private static async Task CheckCSharpClientsAsync(string projectName, string targetFramework, bool generatesCode, string generatedClientsCsPath)
        {
            var code = File.ReadAllText(generatedClientsCsPath);
            code = Regex.Replace(code, "NSwag v.*\\)", "NSwag");
            await Verifier.Verify(code).UseMethodName(nameof(CheckCSharpClientsAsync)).UseParameters(projectName, targetFramework, generatesCode);
        }

        private static async Task CheckTypeScriptAsync(string projectName, string targetFramework, bool generatesCode, string generatedClientsTsPath)
        {
            var code = File.ReadAllText(generatedClientsTsPath);
            code = Regex.Replace(code, "NSwag v.*\\)", "NSwag");
            await Verifier.Verify(code).UseMethodName(nameof(CheckTypeScriptAsync)).UseParameters(projectName, targetFramework, generatesCode);
        }
    }
}