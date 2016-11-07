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
        public class TagsTest1Controller : ApiController
        {

        }

        [TestMethod]
        public void When_controller_has_tag_attributes_then_they_are_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTest1Controller>();

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

        public class TagsTest2Controller : ApiController
        {
            [SwaggerTags("foo", "bar")]
            public void MyAction()
            {

            }
        }

        [TestMethod]
        public void When_operation_has_tags_attributes_then_they_are_processed()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTest2Controller>();

            //// Assert
            Assert.AreEqual("[\"foo\",\"bar\"]", JsonConvert.SerializeObject(service.Operations.First().Operation.Tags));
        }



        // AddToDocument tests

        public class TagsTest3Controller : ApiController
        {
            [SwaggerTag("foo", AddToDocument = true)]
            public void MyAction()
            {

            }
        }

        [TestMethod]
        public void When_operation_has_tag_attribute_with_AddToDocument_then_it_is_added_to_document()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTest3Controller>();

            //// Assert
            Assert.AreEqual(1, service.Tags.Count);
            Assert.AreEqual("foo", service.Tags[0].Name);

            Assert.AreEqual(1, service.Operations.First().Operation.Tags.Count);
            Assert.AreEqual("foo", service.Operations.First().Operation.Tags[0]);
        }

        public class TagsTest4Controller : ApiController
        {
            [SwaggerTags("foo", "bar", AddToDocument = true)]
            public void MyAction()
            {

            }
        }

        [TestMethod]
        public void When_operation_has_tags_attribute_with_AddToDocument_then_it_is_added_to_document()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController<TagsTest4Controller>();

            //// Assert
            Assert.AreEqual(2, service.Tags.Count);
            Assert.AreEqual("foo", service.Tags[0].Name);
            Assert.AreEqual("bar", service.Tags[1].Name);

            Assert.AreEqual(2, service.Operations.First().Operation.Tags.Count);
            Assert.AreEqual("foo", service.Operations.First().Operation.Tags[0]);
            Assert.AreEqual("bar", service.Operations.First().Operation.Tags[1]);
        }
    }
}
