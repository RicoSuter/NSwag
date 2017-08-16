using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]
    public class ControllerGenerationFormatTests
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
        public async Task When_controllergenerationformat_abstract_then_abstractcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyleEnum.Abstract,
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract class TestController"));
            Assert.IsFalse(code.Contains("ITestController"));
            Assert.IsFalse(code.Contains("private ITestController _implementation;"));
            Assert.IsFalse(code.Contains("partial"));
        }

        [TestMethod]
        public async Task When_controllergenerationformat_abstract_then_partialcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyleEnum.Partial,
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("partial class TestController"));
            Assert.IsTrue(code.Contains("ITestController"));
            Assert.IsTrue(code.Contains("private ITestController _implementation;"));
            Assert.IsFalse(code.Contains("abstract class TestController"));
        }

        [TestMethod]
        public async Task When_controllergenerationformat_notsetted_then_partialcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("partial class TestController"));
            Assert.IsTrue(code.Contains("ITestController"));
            Assert.IsTrue(code.Contains("private ITestController _implementation;"));
            Assert.IsFalse(code.Contains("abstract class TestController"));
        }
    }
}


