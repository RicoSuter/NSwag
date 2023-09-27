using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace NSwag.ConsoleCore.Tests
{
    [UsesVerify]
    public class GenerateSampleSpecificationTests
    {
        [Theory]
        [InlineData("NSwag.Sample.NET60", "net6.0")]
        [InlineData("NSwag.Sample.NET60Minimal", "net6.0")]
        [InlineData("NSwag.Sample.NET70", "net7.0")]
        [InlineData("NSwag.Sample.NET70Minimal", "net7.0")]
        public async Task Should_generate_openapi_for_project(string projectName, string targetFramework)
        {
            // Arrange
#if DEBUG
            var executablePath = $"../../../../NSwag.ConsoleCore/bin/Debug/{targetFramework}/dotnet-nswag.dll";
#else
            var executablePath = $"../../../../NSwag.ConsoleCore/bin/Release/{targetFramework}/dotnet-nswag.dll";
#endif
            var nswagJsonPath = $"../../../../{projectName}/nswag.json";
            var openApiJsonPath = $"../../../../{projectName}/openapi.json";

            File.Delete(openApiJsonPath);
            Assert.False(File.Exists(openApiJsonPath));

            // Act
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = executablePath + " run " + nswagJsonPath,
                CreateNoWindow = true,
            });

            process.WaitForExit(20000);
            process.Kill();

            // Assert
            Assert.Equal(0, process.ExitCode);

            var json = File.ReadAllText(openApiJsonPath);
            json = Regex.Replace(json, "\"NSwag v.*\"", "\"NSwag\"");

            await Verifier.Verify(json).UseParameters(projectName, targetFramework);
        }
    }
}