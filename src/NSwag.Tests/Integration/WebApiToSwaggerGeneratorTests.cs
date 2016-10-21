using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Demo.Web.Controllers;

namespace NSwag.Tests.Integration
{
    [TestClass]
    public class WebApiToSwaggerGeneratorTests
    {
        [TestMethod]
        public void When_generating_swagger_from_controller_than_all_required_operations_are_available()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<PersonsController>();
            var swaggerSpecification = service.ToJson();

            //// Assert
            Assert.AreEqual(10, service.Operations.Count());
        }

        [TestMethod]
        public void When_there_is_a_ResultType_attribute_on_an_action_method_then_the_response_is_taken_from_the_given_type()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<PersonsController>();
            var operation = service.Operations.Single(o => o.Path == "/api/Persons/Get/{id}");

            //// Assert
            Assert.AreEqual(2, operation.Operation.Responses.Count);
            Assert.AreEqual("Person", operation.Operation.Responses["200"].ActualResponseSchema.GetTypeName(null, string.Empty));
        }
    }
}
