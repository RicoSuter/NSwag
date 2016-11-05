using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class TagsTests
    {
        [SwaggerTags("x", "y")]
        [SwaggerTag("a1", Description = "a2")]
        [SwaggerTag("b1", Description = "b2", DocumentationDescription = "b3", DocumentationUrl = "b4")]
        public class TagsTestController : ApiController
        {
            [SwaggerTags("foo", "bar")]
            public void MyAction()
            {
                
            }
        }

        [TestMethod]
        public void When_controller_has_tag_attributes_then_they_are_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTestController>();

            //// Assert
            Assert.AreEqual(4, service.Tags.Count);

            Assert.AreEqual("x", service.Tags[0].Name);
            Assert.AreEqual("y", service.Tags[1].Name);

            Assert.AreEqual("a1", service.Tags[2].Name);
            Assert.AreEqual("a2", service.Tags[2].Description);
            Assert.AreEqual(null, service.Tags[2].ExternalDocumentation);

            Assert.AreEqual("b1", service.Tags[3].Name);
            Assert.AreEqual("b2", service.Tags[3].Description);
            Assert.AreEqual("b3", service.Tags[3].ExternalDocumentation.Description);
            Assert.AreEqual("b4", service.Tags[3].ExternalDocumentation.Url);
        }

        [TestMethod]
        public void When_operation_has_tags_attributes_then_they_are_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTestController>();

            //// Assert
            Assert.AreEqual("[\"foo\",\"bar\"]", JsonConvert.SerializeObject(service.Operations.First().Operation.Tags));
        }
    }
}
