using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator.Attributes
{
    // See https://github.com/NSwag/NSwag/issues/48

    [TestClass]
    public class RouteInheritanceTests
    {
        [TestMethod]
        public void When_route_is_on_inherited_parent_class_and_route_prefix_on_class_then_it_is_used_for_swagger_generation()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<MyController>();

            //// Assert
            Assert.AreEqual("/api/My/Foo", service.Operations.First().Path);
        }

        [RoutePrefix("api/My")]
        public class MyController : BaseController
        {

        }

        public class BaseController : ApiController
        {
            [Route("Foo")]
            public string Bar()
            {
                return "foo";
            }
        }

        [TestMethod]
        public void When_route_is_on_inherited_parent_class_then_it_is_used_for_swagger_generation()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<MyController2>();

            //// Assert
            Assert.AreEqual("/Foo", service.Operations.First().Path);
        }

        public class MyController2 : BaseController2
        {

        }

        public class BaseController2 : ApiController
        {
            [Route("Foo")]
            public string Bar()
            {
                return "foo";
            }
        }
    }
}
