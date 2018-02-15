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
        public class ComplexType
        {
            public string Prop1 { get; set; }
            public int Prop2 { get; set; }
            public bool Prop3 { get; set; }
            public ComplexType Prop4 { get; set; }
        }

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

            [Route("Complex")]
            public void Complex([FromBody] ComplexType complexType)
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
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract class TestController"));
            Assert.IsFalse(code.Contains("ITestController"));
            Assert.IsFalse(code.Contains("private ITestController _implementation;"));
            Assert.IsFalse(code.Contains("partial class TestController"));
        }

        [TestMethod]
        public async Task When_controllergenerationformat_abstract_then_partialcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
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
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
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
        public async Task When_controller_has_operation_with_complextype_then_partialcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                AspNetNamespace = "MyCustomNameSpace"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("partial class TestController"));
            Assert.IsTrue(code.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)"));
            Assert.IsTrue(code.Contains("Foo(string test, bool test2)"));
            Assert.IsTrue(code.Contains("Bar()"));
        }

        [TestMethod]
        public async Task When_controller_has_operation_with_complextype_then_abstractcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                AspNetNamespace = "MyCustomNameSpace"
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.IsTrue(code.Contains("abstract class TestController"));
            Assert.IsTrue(code.Contains($"Complex([{settings.AspNetNamespace}.FromBody] ComplexType complexType)"));
            Assert.IsTrue(code.Contains("Foo(string test, bool test2)"));
            Assert.IsTrue(code.Contains("Bar()"));
        }
    }
}


