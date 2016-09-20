using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.Integration.ClientPCL.Tests
{
    [TestClass]
    public class PersonsControllerTests
    {
        [TestMethod]
        [TestCategory("integration")]
        public async Task GetAll()
        {
            //// Arrange
            var personsClient = new PersonsClient("http://localhost:13452");

            //// Act
            var persons = await personsClient.GetAllAsync();

            //// Assert
            Assert.AreEqual(2, persons.Count);
        }
    }
}
