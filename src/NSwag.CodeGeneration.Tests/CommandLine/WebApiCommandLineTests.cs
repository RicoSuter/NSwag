using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.CodeGeneration.Tests.CommandLine
{
    [TestClass]
    public class WebApiCommandLineTests
    {
        [TestMethod]
        public void When_webapi2swagger_is_called_then_file_is_created()
        {
            //// Arrange
            var command = "webapi2swagger " +
                          "/assembly:" + Path.GetFullPath("../../../NSwag.Demo.Web/bin/NSwag.Demo.Web.dll") + " " +
                          "/controller:NSwag.Demo.Web.Controllers.PersonsController " +
                          "/defaultUrlTemplate:api/{controller}/{action}/{id} " +
                          "/output:" + OutputFile;

            //// Act
            var output = RunCommandLine(command);
            var document = SwaggerDocument.FromJson(output);

            //// Assert
            Assert.IsNotNull(document);
        }

        [TestMethod]
        public void When_swagger2typescript_is_called_then_file_is_created()
        {
            //// Arrange
            var command = "swagger2tsclient " +
                          @"/input:""{ \""swagger\"": \""2.0\"", \""paths\"": {}, \""definitions\"": { \""Test\"": { typeName: \""Test\"", type: \""Object\"" } } }"" " +
                          "/output:" + OutputFile;

            //// Act
            var output = RunCommandLine(command);

            //// Assert
            Assert.IsTrue(output.Contains("export class Test {"));
        }

        private const string OutputFile = "Output.json";

        private static string RunCommandLine(string command)
        {
            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            var configuration = Directory.GetCurrentDirectory().Contains("bin\\Release") ? "Release" : "Debug";
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetFullPath("../../../NSwag.Console/bin/" + configuration + "/NSwag.exe"),
                Arguments = command,
                CreateNoWindow = true, 
                WindowStyle = ProcessWindowStyle.Hidden
            });

            if (!process.WaitForExit(5000))
            {
                process.Kill();
                throw new InvalidOperationException("The process did not terminate.");
            }

            var output = File.ReadAllText(OutputFile);

            if (File.Exists(OutputFile))
                File.Delete(OutputFile);

            return output;
        }
    }
}
