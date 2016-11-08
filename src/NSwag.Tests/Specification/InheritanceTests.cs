using System.Text.RegularExpressions;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.Tests.Specification
{
    [TestClass]
    public class InheritanceTests
    {
        [TestMethod]
        public void When_generating_type_with_deep_inheritance_then_allOf_has_one_item()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<TestController>();
            var swaggerSpecification = document.ToJson();

            //// Assert
            Assert.AreEqual(2, Regex.Matches(Regex.Escape(swaggerSpecification), "allOf").Count); // must have an allOf in BB and CC, no more (rest are refs)
        }

        public class TestController : ApiController
        {
            public void Post([FromBody]CC value)
            {
            }
        }

        public class AA
        {
            public string FirstName { get; set; }
        }

        public class BB : AA
        {
            public string LastName { get; set; }
        }

        public class CC : BB
        {
            public string Address { get; set; }
        }
    }
}
