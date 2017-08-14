using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class GenerateAbstractControllersTests
    {
        public class TestController : ApiController
        {
            [Route("Foo")]
            public string Foo()
            {
                throw new NotImplementedException();
            }

            [Route("Bar")]
            public void Bar()
            {
                throw new NotImplementedException();
            }
        }
        [TestMethod]
        public async Task When_generateabstractcontrollers_true_then_abstractcontroller_is_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateControllerInterfaces = false,
                GenerateAbstractControllers = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract class TestController"));
            Assert.IsTrue(code.Contains("abstract Foo"));
        }

        [TestMethod]
        public async Task When_generateabstractcontrollers_false_then_abstractcontroller_is_not_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateControllerInterfaces = true,
                GenerateAbstractControllers = false
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract class TestController"));
            Assert.IsTrue(code.Contains("abstract Foo"));
        }
    }
}


