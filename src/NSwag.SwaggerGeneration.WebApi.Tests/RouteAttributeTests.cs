using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.SwaggerGeneration.WebApi.Tests
{
    [TestClass]
    public class RouteAttributeTests
    {
       [Route("/api/[controller]")]
       public abstract class BaseTestController : ApiController
       {
       }

        public class FooController : BaseTestController
        {
            public void Post()
            {
            }
        }

        [Route("/api/other")]
        public class BarController : BaseTestController
        {
            public void Post()
            {
            }
        }

        [TestMethod]
        public async Task Use_base_class_route_attribute()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<FooController>();
            var swaggerSpecification = document.ToJson();

            StringAssert.Contains(swaggerSpecification, "\"/api/Foo\"");
        }

        [TestMethod]
        public async Task Route_attribute_overrides_base()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<BarController>();
            var swaggerSpecification = document.ToJson();

            StringAssert.Contains(swaggerSpecification, "\"/api/other\"");
        }
    }
}
