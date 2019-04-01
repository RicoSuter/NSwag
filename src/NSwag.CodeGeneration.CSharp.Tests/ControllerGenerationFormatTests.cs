using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.CodeGeneration.CSharp.Models;
using NSwag.SwaggerGeneration.WebApi;
using Xunit;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ControllerGenerationFormatTests
    {
        public class ComplexType
        {
            public string Prop1 { get; set; }

            public int Prop2 { get; set; }

            public bool Prop3 { get; set; }

            public ComplexType Prop4 { get; set; }
        }

        public class TestController : Controller
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

        [Fact]
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
            Assert.Contains("abstract class TestController", code);
            Assert.DoesNotContain("ITestController", code);
            Assert.DoesNotContain("private ITestController _implementation;", code);
            Assert.DoesNotContain("partial class TestController", code);
        }

        [Fact]
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
            Assert.Contains("partial class TestController", code);
            Assert.Contains("ITestController", code);
            Assert.Contains("private ITestController _implementation;", code);
            Assert.DoesNotContain("abstract class TestController", code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_abstract_then_actiontype_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("public abstract System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> Foo(string test, bool test2);", code);
            Assert.Contains("public abstract System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> Bar();", code);
        }

        [Fact]
        public async Task When_aspnet_actiontype_inuse_with_partial_then_actiontype_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Partial,
                UseActionResultType = true
            });
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> FooAsync(string test, bool test2);", code);
            Assert.Contains("public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.ActionResult<string>> Foo(string test, bool test2)", code);
            Assert.Contains("System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> BarAsync();", code);
            Assert.Contains("public System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> Bar()", code);
        }

        [Fact]
        public async Task When_controllergenerationformat_notsetted_then_partialcontroller_is_generated()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, new SwaggerToCSharpControllerGeneratorSettings());
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains("ITestController", code);
            Assert.Contains("private ITestController _implementation;", code);
            Assert.DoesNotContain("abstract class TestController", code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_partialcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings();

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("partial class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains("Foo(string test, bool test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_controller_has_operation_with_complextype_then_abstractcontroller_is_generated_with_frombody_attribute()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                ControllerStyle = CSharpControllerStyle.Abstract,
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("abstract class TestController", code);
            Assert.Contains($"Complex([Microsoft.AspNetCore.Mvc.FromBody] ComplexType complexType)", code);
            Assert.Contains("Foo(string test, bool test2)", code);
            Assert.Contains("Bar()", code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_operationid_then_route_attribute_name_specified()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.OperationId
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("Route(\"Foo\", Name = \"Test_Foo\")", code);
            Assert.Contains("Route(\"Bar\", Name = \"Test_Bar\")", code);
        }

        [Fact]
        public async Task When_controllerroutenamingstrategy_none_then_route_attribute_name_not_specified()
        {
            //// Arrange
            var swaggerGen = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings());
            var document = await swaggerGen.GenerateForControllerAsync<TestController>();
            var settings = new SwaggerToCSharpControllerGeneratorSettings
            {
                RouteNamingStrategy = CSharpControllerRouteNamingStrategy.None
            };

            //// Act
            var codeGen = new SwaggerToCSharpControllerGenerator(document, settings);
            var code = codeGen.GenerateFile();

            //// Assert
            Assert.Contains("Route(\"Foo\")", code);
            Assert.Contains("Route(\"Bar\")", code);
        }
    }
}


