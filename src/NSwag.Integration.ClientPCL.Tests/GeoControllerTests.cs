using System;
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
            catch (SwaggerException exception)
            {
                Assert.IsTrue(exception.InnerException is ArgumentException);
                Assert.IsTrue(exception.InnerException.StackTrace.Contains("NSwag.Integration.WebAPI.Controllers.GeoController.SaveItems"));
            }
        }
    }
}