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
    public class GenerateControllerInterfacesTests
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
        public async Task When_generatecontrollerinterfaces_true_then_interface_is_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateControllerInterfaces = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("ITestController"));
            Assert.IsTrue(code.Contains("private ITestController _implementation; "));
        }

        [TestMethod]
        public async Task When_generatecontrollerinterfaces_false_then_interface_is_not_returned()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpClientGenerator(document, new SwaggerToCSharpClientGeneratorSettings
            {
                GenerateControllerInterfaces = false
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsFalse(code.Contains("ITestController"));
            Assert.IsFalse(code.Contains("private ITestController _implementation; "));
        }
    }
}
