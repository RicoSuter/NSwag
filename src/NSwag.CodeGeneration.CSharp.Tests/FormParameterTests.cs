using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class FormParameterTests
    {
        [TestMethod]
        public void When_form_parameters_are_defined_then_MultipartFormDataContent_is_generated()
        {
            //// Arrange
            var document = new SwaggerDocument();
            document.Paths["foo/bar"] = new SwaggerPathItem
            {
                {
                    SwaggerOperationMethod.Post,
                    new SwaggerOperation
                    {
                        Parameters = 
                        {
                            new SwaggerParameter
                            {
                                Name = "foo",
                                IsRequired = false,
                                IsNullableRaw = true,
                                Kind = SwaggerParameterKind.FormData,
                                Type = JsonObjectType.String
                            },
                            new SwaggerParameter
                            {
                                Name = "bar",
                                IsRequired = true,
                                IsNullableRaw = false, 
                                Kind = SwaggerParameterKind.FormData,
                                Type = JsonObjectType.String
                            }
                        }
                    }
                }
            };

            //// Act
            var generator = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = generator.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("new System.Net.Http.MultipartFormDataContent"));
            Assert.IsTrue(code.Contains("if (foo != null)"));
            Assert.IsTrue(code.Contains("throw new System.ArgumentNullException(\"bar\");"));
        }

        public class FileUploadController : ApiController
        {
            public void Upload(HttpPostedFileBase file)
            {
            }
        }

        [TestMethod]
        public async Task When_action_has_file_parameter_then_Stream_is_generated_in_CSharp_code()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await generator.GenerateForControllerAsync<FileUploadController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("FileParameter file"));
            Assert.IsTrue(code.Contains("var content_ = new System.Net.Http.MultipartFormDataContent(boundary_);"));
            Assert.IsTrue(code.Contains("content_.Add(new System.Net.Http.StreamContent(file.Data), \"file\""));
        }

        // TODO: Implement for JQuery, AngularJS and Angular 2

        //[TestMethod]
        //public void When_action_has_file_parameter_then_Stream_is_generated_in_TypeScript_code()
        //{
        //    //// Arrange
        //    var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
        //    var service = generator.GenerateForController<FileUploadController>();

        //    //// Act
        //    var codeGen = new SwaggerToTypeScriptClientGenerator(service, new SwaggerToTypeScriptClientGeneratorSettings());
        //    var code = codeGen.GenerateFile();

        //    //// Assert
        //    Assert.IsTrue(code.Contains("Stream file"));
        //    Assert.IsTrue(code.Contains("var content_ = new MultipartFormDataContent();"));
        //    Assert.IsTrue(code.Contains("content_.Add(new StreamContent(file), \"file\");"));
        //}
    }
}