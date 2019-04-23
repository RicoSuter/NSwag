using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Commands;

namespace NSwag.Tests.CommandLine
{
    [TestClass]
    public class WebApiCommandLineTests
    {
        private const string DefaultOutputPath = "../../Output.json";

        [TestMethod]
        public async Task When_webapi2swagger_is_called_then_file_is_created()
        {
            //// Arrange
            var command = "webapi2swagger " +
                          "/assembly:" + Path.GetFullPath("../../../NSwag.Demo.Web/bin/NSwag.Demo.Web.dll") + " " +
                          "/controller:NSwag.Demo.Web.Controllers.PersonsDefaultRouteController " +
                          "/defaultUrlTemplate:api/{controller}/{action}/{id} " +
                          "/output:" + DefaultOutputPath;

            //// Act
            var output = RunCommandLine(command, DefaultOutputPath);
            var document = await SwaggerDocument.FromJsonAsync(output);

            //// Assert
            Assert.IsNotNull(document);
        }

        [TestMethod]
        public void When_swagger2typescript_is_called_then_file_is_created()
        {
            //// Arrange
            var command = "swagger2tsclient " +
                          @"/input:""{ \""swagger\"": \""2.0\"", \""paths\"": {}, \""definitions\"": { \""Test\"": { type: \""object\"", properties: { \""Foo\"": { type: \""string\"" } } } } }"" " +
                          "/output:" + DefaultOutputPath;

            //// Act
            var output = RunCommandLine(command, DefaultOutputPath);

            //// Assert
            Assert.IsTrue(output.Contains("export class Test implements ITest {"));
        }

        //[TestMethod]
        public async Task When_config_file_with_project_with_newer_json_net_is_run_then_property_is_correct()
        {
            //// Arrange
            var command = "run \"" + Path.GetFullPath("../../../NSwag.VersionMissmatchTest/nswag.json") + "\" /runtime:WinX64";

            //// Act
            var output = RunCommandLine(command, Path.GetFullPath("../../../NSwag.VersionMissmatchTest/output.json"));
            var document = await SwaggerDocument.FromJsonAsync(output);

            //// Assert
            var json = document.ToJson();
            Assert.IsTrue(json.Contains("\"Bar\": {"));
        }

        //[TestMethod]
        public async Task RunIntegrationTests()
        {
            //// Arrange
            foreach (var path in Directory.GetDirectories("../../../NSwag.Integration.Tests"))
            {
                try
                {
                    var configPath = Path.GetFullPath(path + "/nswag.json");
                    var config = await NSwagDocument.LoadAsync(configPath);
                    var outputPath = config.SwaggerGenerators.WebApiToSwaggerCommand.OutputFilePath;

                    //// Act
                    var command = "run \"" + configPath + "\"";
                    var output = RunCommandLine(command, outputPath);
                    var document = await SwaggerDocument.FromJsonAsync(output);

                    //// Assert
                    Assert.IsTrue(document.Paths.Any());
                    Debug.WriteLine("The integration test '" + Path.GetFileName(path) + "' passed!");
                }
                catch (Exception e)
                {
                    throw new Exception("The integration test '" + Path.GetFileName(path) + "' failed: " + e);
                }
            }
        }

        private static string RunCommandLine(string command, string outputPath)
        {
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            var configuration = Directory.GetCurrentDirectory().Contains("bin\\Release") ? "Release" : "Debug";
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetFullPath("../../../NSwag.Console/bin/" + configuration + "/net461/NSwag.exe"),
                Arguments = command,
                //CreateNoWindow = true, 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                //WindowStyle = ProcessWindowStyle.Hidden
            });

            if (!process.WaitForExit(10000))
            {
                var cmdOutput2 = process.StandardOutput.ReadToEnd();
                Console.WriteLine(cmdOutput2);

                process.Kill();
                throw new InvalidOperationException("The process did not terminate.");
            }

            var cmdOutput = process.StandardOutput.ReadToEnd();
            Console.WriteLine(cmdOutput);

            var output = File.ReadAllText(outputPath);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            return output;
        }
    }
}
