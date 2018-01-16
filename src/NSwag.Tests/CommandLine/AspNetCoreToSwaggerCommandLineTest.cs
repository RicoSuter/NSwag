using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Tests.CommandLine
{
    [TestClass]
    public class AspNetCoreToSwaggerCommandLineTest
    {
        private static readonly string DefaultOutputPath = Path.GetFullPath("../../Output.json");

        [TestMethod]
        public async Task When_aspnetcore2swagger_is_called_then_file_is_created()
        {
            //// Arrange
            var projectPath = Path.GetFullPath("../../../NSwag.Sample.NetCoreAngular/NSwag.Sample.NetCoreAngular.csproj");
            var command = $"aspnetcore2swagger /project:\"{projectPath}\" /output:\"{DefaultOutputPath}\" /verbose:true";

            //// Act
            var output = RunCommandLine(command, DefaultOutputPath);
            var document = await SwaggerDocument.FromJsonAsync(output);

            //// Assert
            Assert.IsNotNull(document);
        }

        private static string RunCommandLine(string command, string outputPath)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            var configuration = Directory.GetCurrentDirectory().Contains("bin\\Release") ? "Release" : "Debug";
            var binaryPath = Path.GetFullPath("../../../NSwag.ConsoleCore/bin/" + configuration + "/netcoreapp1.0/dotnet-nswag.dll");
            command = binaryPath + " " + command;

            command += $" /configuration:{configuration}";

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            });

            if (!process.WaitForExit((int)TimeSpan.FromMinutes(3).TotalMilliseconds))
            {
                process.Kill();
                throw new InvalidOperationException("The process did not terminate.");
            }

            var standardOut = process.StandardOutput.ReadToEnd();
            var standardError = process.StandardError.ReadToEnd();
            if (process.ExitCode != 0 || !string.IsNullOrEmpty(standardError))
            {
                throw new Exception(
$@"Process failed with non-zero exit code {process.ExitCode}.
====================
Standard Out: {standardOut}
====================
Standard Error: {standardError}.");
            }

            var output = File.ReadAllText(outputPath);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            return output;
        }
    }
}
