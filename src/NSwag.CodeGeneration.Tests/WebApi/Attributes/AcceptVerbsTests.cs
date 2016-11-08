using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class AcceptVerbsTests
    {
        public class TestController : ApiController
        {
            [AcceptVerbs("Post")]
            public int AddPost(int a, int b)
            {
                return a + b; 
            }

            [AcceptVerbs("Get")]
            public int AddGet(int a, int b)
            {
                return a + b;
            }

            [AcceptVerbs("Delete")]
            public int AddDelete(int a, int b)
            {
                return a + b;
            }

            [AcceptVerbs("Put")]
            public int AddPut(int a, int b)
            {
                return a + b;
            }
        }

        [TestMethod]
        public void When_accept_verbs_attribute_with_post_is_used_then_http_method_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.First(o => o.Operation.OperationId == "Test_AddPost");

            //// Assert
            Assert.AreEqual(SwaggerOperationMethod.Post, operation.Method);
        }

        [TestMethod]
        public void When_accept_verbs_attribute_with_get_is_used_then_http_method_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.First(o => o.Operation.OperationId == "Test_AddGet");

            //// Assert
            Assert.AreEqual(SwaggerOperationMethod.Get, operation.Method);
        }

        [TestMethod]
        public void When_accept_verbs_attribute_with_delete_is_used_then_http_method_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.First(o => o.Operation.OperationId == "Test_AddDelete");

            //// Assert
            Assert.AreEqual(SwaggerOperationMethod.Delete, operation.Method);
        }

        [TestMethod]
        public void When_accept_verbs_attribute_with_put_is_used_then_http_method_is_correct()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());

            //// Act
            var document = generator.GenerateForController<TestController>();
            var operation = document.Operations.First(o => o.Operation.OperationId == "Test_AddPut");

            //// Assert
            Assert.AreEqual(SwaggerOperationMethod.Put, operation.Method);
        }
    }
}