using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi
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
                AssemblyPaths = new[] { @"./NSwag.CodeGeneration.Tests.dll" },
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            };

            var generator = new WebApiAssemblyToSwaggerGenerator(settings);

            //// Act
            var document = generator.GenerateForControllers(new[] { "NonExistingClass" }); // Should throw exception

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
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController(typeof(FromUriFileParameterController));

            //// Assert
            var operation = document.Paths["/upload"][SwaggerOperationMethod.Post];

            Assert.AreEqual(JsonObjectType.File, operation.ActualParameters.Single(p => p.Name == "formFile").Type);
            Assert.IsTrue(operation.ActualParameters.Any(p => p.Name == "formFile"));
            Assert.IsTrue(operation.ActualParameters.Any(p => p.Name == "CustomLocationToSave"));
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
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController(typeof(FileCollectionController));

            //// Assert
            var operation = document.Paths["/upload"][SwaggerOperationMethod.Post];
            var parameter = operation.ActualParameters.Single(p => p.Name == "files");

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
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController(typeof(FromUriParameterController));

            //// Assert
            var operation = document.Paths["/upload"][SwaggerOperationMethod.Post];

            Assert.AreEqual(JsonObjectType.String, operation.ActualParameters.Single(p => p.Name == "Foo").Type);
            Assert.AreEqual(JsonObjectType.String, operation.ActualParameters.Single(p => p.Name == "Bar").Type);

            Assert.IsTrue(operation.ActualParameters.Any(p => p.Name == "Foo"));
            Assert.IsTrue(operation.ActualParameters.Any(p => p.Name == "Bar"));

            Assert.IsNull(operation.Consumes);
        }

        public class ConstrainedRoutePathController : ApiController
        {
            [Route("{id:long:min(1)}")]
            public object Get(long id)
            {
                return null;
            }
        }

        [TestMethod]
        public void When_web_api_path_has_constraints_then_they_are_removed_in_the_swagger_spec()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController(typeof(ConstrainedRoutePathController));

            //// Assert
            var path = document.Paths.First().Key;

            Assert.AreEqual("/{id}", path);
        }

        [Route("account/{action}/{id?}")]
        public class AccountController : ApiController
        {
            [HttpGet]
            public string Get()
            {
                return null;
            }

            [HttpGet]
            public string GetAll()
            {
                return null;
            }

            [HttpPost]
            public async Task<IHttpActionResult> Post([FromBody] object model)
            {
                return null;
            }

            [HttpPost]
            public async Task<IHttpActionResult> Verify([FromBody] object model)
            {
                return null;
            }

            [HttpPost]
            public async Task<IHttpActionResult> Confirm([FromBody] object model)
            {
                return null;
            }
        }

        [TestMethod]
        public void When_class_has_RouteAttribute_with_placeholders_then_they_are_correctly_replaced()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController(typeof(AccountController));

            //// Assert
            Assert.IsTrue(document.Paths.ContainsKey("/account/Get"));
            Assert.IsTrue(document.Paths.ContainsKey("/account/GetAll"));
            Assert.IsTrue(document.Paths.ContainsKey("/account/Post"));
            Assert.IsTrue(document.Paths.ContainsKey("/account/Verify"));
            Assert.IsTrue(document.Paths.ContainsKey("/account/Confirm"));
        }
    }
}
