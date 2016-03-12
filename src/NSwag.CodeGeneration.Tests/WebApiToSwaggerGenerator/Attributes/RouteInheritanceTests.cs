using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator.Attributes
{
    // See https://github.com/NSwag/NSwag/issues/48

    //[TestClass]
    public class RouteInheritanceTests
    {
        //[TestMethod]
        public void When_route_is_on_inherited_parent_class_then_it_is_used_for_swagger_generation()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<MyController>();

            //// Assert
            Assert.AreEqual("api/My/Foo", service.Operations.First().Path);
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
    }
}
