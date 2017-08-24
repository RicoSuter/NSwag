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
            public string Foo(string test, bool test2)
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
                ControllerStyle = CSharpControllerStyle.Abstract,
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
                ControllerStyle = CSharpControllerStyle.Partial,
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

        [TestMethod]
        public async Task When_partialcontrollermethodwithnorequestparameter_is_async_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("(System.Threading.CancellationToken cancellationToken)"));
            Assert.IsTrue(code.Contains("_implementation.BarAsync(cancellationToken)"));
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task BarAsync(System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_partialcontrollermethodhasrequestparameter_is_async_then_cancellationtoken_is_added()
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
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task<string> FooAsync(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
            Assert.IsTrue(code.Contains("_implementation.FooAsync(test, test2, cancellationToken);"));
            Assert.IsTrue(code.Contains("public System.Threading.Tasks.Task<string> Foo(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_abstractcontrollermethodwithnorequestparameter_is_async_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract System.Threading.Tasks.Task Bar(System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_abstractcontrollermethodhasrequestparameter_is_async_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpWebApiControllerGenerator(document, new SwaggerToCSharpWebApiControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task<string> Foo(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
        }

    }
}


