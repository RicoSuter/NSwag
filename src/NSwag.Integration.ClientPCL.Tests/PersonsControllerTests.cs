using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.Integration.ClientPCL.Contracts;

namespace NSwag.Integration.ClientPCL.Tests
{
    [TestClass]
    public class PersonsControllerTests
    {
        [TestMethod]
        [TestCategory("integration")]
        public async Task GetAll_SerializationTest()
        {
            //// Arrange
            var personsClient = new PersonsClient(new HttpClient()) { BaseUrl = "http://localhost:13452" }; ;

            //// Act
            var persons = await personsClient.GetAllAsync();

            //// Assert
            Assert.AreEqual(2, persons.Result.Count);
            Assert.IsTrue(persons.Result[0].GetType() == typeof(Person));
            Assert.IsTrue(persons.Result[1].GetType() == typeof(Teacher));
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task AddXml_PostXml()
        {
            //// Arrange
            var personsClient = new PersonsClient(new HttpClient()) { BaseUrl = "http://localhost:13452" }; ;

            //// Act
            var result = await personsClient.AddXmlAsync("<Rico>Suter</Rico>");

            //// Assert
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task GetAll_InheritanceTest()
        {
            //// Arrange
            var personsClient = new PersonsClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            var persons = await personsClient.GetAllAsync();

            //// Assert
            Assert.AreEqual("SE", ((Teacher)persons.Result[1]).Course); // inheritance test
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task Throw()
        {
            //// Arrange
            var id = Guid.NewGuid();
            var personsClient = new PersonsClient(new HttpClient()) { BaseUrl = "http://localhost:13452" };

            //// Act
            try
            {
                var persons = await personsClient.ThrowAsync(id);
            }
            catch (PersonsClientException<PersonNotFoundException> exception)
            {
                //// Assert
                Assert.AreEqual(id, exception.Result.Id);
            }
        }

        [TestMethod]
        [TestCategory("integration")]
        public async Task Get_should_return_teacher()
        {
            //// Arrange
            var personsClient = new PersonsClient(new HttpClient()) { BaseUrl = "http://localhost:13452" }; ;

            //// Act
            var result = await personsClient.GetAsync(new Guid());

            //// Assert
            Assert.IsTrue(result.Result is Teacher);
        }

    }
}
