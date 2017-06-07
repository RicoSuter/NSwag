using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NSwag.SwaggerGeneration.WebApi.Tests
{
    [TestClass]
    public class RouteAttributeTests
    {
        [AttributeUsage(AttributeTargets.Class)]
        internal class CustomRouteAttribute : Attribute, IHttpRouteInfoProvider
        {
            public CustomRouteAttribute(string template)
            {
                Template = template;
            }

            public string Template { get; }
            public int Order { get; } = 0;
            public string Name { get; set; }
        }

        [Route("api/[controller]")]
        [CustomRoute(null)]
        public class SkipNullController : ApiController
        {
            public void Post()
            {
            }
        }

        [CustomRoute("api/[controller]")]
        public class MyApiController : ApiController
        {
            public void Post()
            {
            }
        }

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

        [TestMethod]
        public async Task Custom_internal_route_attribute()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<MyApiController>();
            var swaggerSpecification = document.ToJson();

            StringAssert.Contains(swaggerSpecification, "\"/api/MyApi\"");
        }

        [TestMethod]
        public async Task Skip_null_route_attribute_temlates()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<SkipNullController>();
            var swaggerSpecification = document.ToJson();

            StringAssert.Contains(swaggerSpecification, "\"/api/SkipNull\"");
        }
    }
}
