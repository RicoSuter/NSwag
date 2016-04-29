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
            if (File.Exists("MyWebService.json"))
                File.Delete("MyWebService.json");

            var command = "webapi2swagger " +
                          "/assembly:" + Path.GetFullPath("../../../NSwag.Demo.Web/bin/NSwag.Demo.Web.dll") + " " +
                          "/controller:NSwag.Demo.Web.Controllers.PersonsController " +
                          "/output:MyWebService.json";

            //// Act
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetFullPath("../../../NSwag/bin/" + (Directory.GetCurrentDirectory().Contains("bin\\Release") ? "Release" : "Debug") + "/NSwag.exe"),
                Arguments = command,
            });

            if (!process.WaitForExit(5000))
                process.Kill();

            var json = File.ReadAllText("MyWebService.json");

            //// Assert
            Assert.IsTrue(!string.IsNullOrEmpty(json));
        }
    }
}
