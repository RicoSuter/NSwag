using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.SwaggerGeneration.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    [TestClass]

    public class UseCancellationTokenTests
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
        public async Task When_controllerstyleispartial_and_usecancellationtokenistrue_and_requesthasnoparameter_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("(System.Threading.CancellationToken cancellationToken)"));
            Assert.IsTrue(code.Contains("_implementation.BarAsync(cancellationToken)"));
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task BarAsync(System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_controllerstyleispartial_and_usecancellationtokenistrue_and_requesthasparameter_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task<string> FooAsync(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
            Assert.IsTrue(code.Contains("_implementation.FooAsync(test, test2, cancellationToken);"));
            Assert.IsTrue(code.Contains("public System.Threading.Tasks.Task<string> Foo(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_controllerstyleisabstract_and_usecancellationtokenistrue_and_requesthasnoparameter_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract System.Threading.Tasks.Task Bar(System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_controllerstyleisabstract_and_usecancellationtokenistrue_and_requesthasparameter_then_cancellationtoken_is_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseCancellationToken = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("System.Threading.Tasks.Task<string> Foo(string test, bool test2, System.Threading.CancellationToken cancellationToken)"));
        }

        [TestMethod]
        public async Task When_usecancellationtokenparameter_notsetted_then_cancellationtoken_isnot_added()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsFalse(code.Contains("System.Threading.CancellationToken cancellationToken"));
        }
    }
}
