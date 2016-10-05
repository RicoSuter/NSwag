using System;
using System.IO;
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
            var geoClient = new GeoClient("http://localhost:13452");

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
            var geoClient = new GeoClient("http://localhost:13452");

            //// Act
            var result = await geoClient.UploadFileAsync(new FileParameter(new MemoryStream(new byte[] { 1, 2 })));

            //// Assert
            Assert.IsTrue(result);
        }
    }
}