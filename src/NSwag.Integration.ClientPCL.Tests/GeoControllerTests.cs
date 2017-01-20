using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Integration.ClientPCL.Contracts;

namespace NSwag.Integration.ClientPCL.Tests
{
    [TestClass]
    public class GeoControllerTests
    {
        [TestMethod]
        [TestCategory("integration")]
        public async Task SaveItems()
        {
            //// Arrange
            var geoClient = new GeoClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            try
            {
                await geoClient.SaveItemsAsync(null);

                //// Assert
                Assert.Fail();
            }
            catch (GeoClientException exception)
            {
                Assert.IsTrue(exception.InnerException is ArgumentException);
                Assert.IsTrue(exception.InnerException.StackTrace.Contains("NSwag.Integration.WebAPI.Controllers.GeoController.SaveItems"));
            }
        }

        //[TestMethod]
        [TestCategory("integration")]
        public async Task UploadFile()
        {
            //// Arrange
            var geoClient = new GeoClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            var result = await geoClient.UploadFileAsync(new FileParameter(new MemoryStream(new byte[] { 1, 2 })));

            //// Assert
            Assert.IsTrue(result.Result);
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task QueryStringParameters()
        {
            //// Arrange
            var geoClient = new GeoClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            var result = await geoClient.ReverseAsync(new string[] { "foo", "bar" });

            //// Assert
            Assert.AreEqual(2, result.Result.Count);
            Assert.AreEqual("foo", result.Result[1]);
            Assert.AreEqual("bar", result.Result[0]);
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task FileDownload()
        {
            //// Arrange
            var geoClient = new GeoClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            using (var response = await geoClient.GetUploadedFileAsync(1, true))
            {
                //// Assert
                Assert.AreEqual(1, response.Stream.ReadByte());
                Assert.AreEqual(2, response.Stream.ReadByte());
                Assert.AreEqual(3, response.Stream.ReadByte());
            }
        }
    }
}