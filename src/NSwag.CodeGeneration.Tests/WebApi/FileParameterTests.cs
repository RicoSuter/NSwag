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
    public class FileParameterTests
    {
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
        public async Task When_parameter_is_from_uri_and_has_file_then_two_params_and_consumes_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync(typeof(FromUriFileParameterController));
            var json = document.ToJson();

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
        public async Task When_parameter_is_file_collection_then_type_is_correct_and_collection_format_is_multi()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync(typeof(FileCollectionController));

            //// Assert
            var operation = document.Paths["/upload"][SwaggerOperationMethod.Post];
            var parameter = operation.ActualParameters.Single(p => p.Name == "files");

            Assert.AreEqual(JsonObjectType.File, parameter.Type);
            Assert.AreEqual(SwaggerParameterCollectionFormat.Multi, parameter.CollectionFormat);

            Assert.AreEqual("multipart/form-data", operation.Consumes[0]);
        }
    }
}
