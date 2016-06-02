using System;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator
{
    [TestClass]
    public class GeneralTests
    {
        [TestMethod]
        [ExpectedException(typeof(TypeLoadException))]
        public void When_controller_type_is_not_found_then_type_load_exception_is_thrown()
        {
            //// Arrange
            var settings = new WebApiAssemblyToSwaggerGeneratorSettings
            {
                AssemblyPath = @"./NSwag.CodeGeneration.Tests.dll",
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            var generator = new WebApiAssemblyToSwaggerGenerator(settings);

            //// Act
            var swaggerService = generator.GenerateForController("NonExistingClass"); // Should throw exception

            //// Assert
        }

        public interface IFormFile
        {
        }

        public interface IActionResult
        {
        }

        public class FromUriFileParameterController : ApiController
        {
            public class ComplexClass
            {
                [JsonProperty("formFile")]
                public IFormFile FormFile { get; set; }

                public string CustomLocationToSave { get; set; }
            }

            [HttpPost, Route("upload")]
            public IActionResult Upload([FromUri] ComplexClass parameters)
            {
                // TODO: Check if this is the correct behavior or if FromUri can be omitted when a property is a file
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void When_parameter_is_from_uri_and_has_file_then_two_params_and_consumes_is_correct()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController(typeof(FromUriFileParameterController));

            //// Assert
            var operation = service.Paths["/upload"][SwaggerOperationMethod.Post];

            Assert.AreEqual(JsonObjectType.File, operation.Parameters.Single(p => p.Name == "formFile").Type);
            Assert.IsTrue(operation.Parameters.Any(p => p.Name == "formFile"));
            Assert.IsTrue(operation.Parameters.Any(p => p.Name == "CustomLocationToSave"));
            Assert.AreEqual("multipart/form-data", operation.Consumes[0]);
        }

        public interface IFormFileCollection
        {
        }

        public class FileCollectionController : ApiController
        {
            [HttpPost, Route("upload")]
            public IActionResult Upload(IFormFileCollection files)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void When_parameter_is_file_collection_then_type_is_correct_and_collection_format_is_multi()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController(typeof(FileCollectionController));

            //// Assert
            var operation = service.Paths["/upload"][SwaggerOperationMethod.Post];
            var parameter = operation.Parameters.Single(p => p.Name == "files");

            Assert.AreEqual(JsonObjectType.File, parameter.Type);
            Assert.AreEqual(SwaggerParameterCollectionFormat.Multi, parameter.CollectionFormat);

            Assert.AreEqual("multipart/form-data", operation.Consumes[0]);
        }

        public class FromUriParameterController : ApiController
        {
            public class ComplexClass
            {
                public string Foo { get; set; }

                public string Bar { get; set; }
            }

            [HttpPost, Route("upload")]
            public IActionResult Upload([FromUri]ComplexClass parameters)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void When_parameter_is_from_uri_then_two_params_are_generated()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var service = generator.GenerateForController(typeof(FromUriParameterController));

            //// Assert
            var operation = service.Paths["/upload"][SwaggerOperationMethod.Post];

            Assert.AreEqual(JsonObjectType.String, operation.Parameters.Single(p => p.Name == "Foo").Type);
            Assert.AreEqual(JsonObjectType.String, operation.Parameters.Single(p => p.Name == "Bar").Type);

            Assert.IsTrue(operation.Parameters.Any(p => p.Name == "Foo"));
            Assert.IsTrue(operation.Parameters.Any(p => p.Name == "Bar"));

            Assert.IsNull(operation.Consumes);
        }
    }
}
