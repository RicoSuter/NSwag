using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class ComplexParametersTests
    {
        public class MyParameter
        {
            public string Foo { get; set; }

            /// <summary>My comment.</summary>
            [JsonProperty("bar")]
            public string Bar { get; set; }
        }

        public class TestController : ApiController
        {
            public string WithoutAttribute(MyParameter data)
            {
                return string.Empty;
            }

            /// <summary>My comment.</summary>
            /// <remarks>My remarks.</remarks>
            public string WithFromUriAttribute([FromUri] MyParameter data)
            {
                return string.Empty;
            }

            public string WithFromBodyAttribute([FromBody] MyParameter data)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public async Task When_parameter_is_complex_then_it_is_a_body_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithoutAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Body, operation.ActualParameters[0].Kind);
            Assert.AreEqual("data", operation.ActualParameters[0].Name);
        }

        [TestMethod]
        public async Task When_parameter_is_complex_and_has_FromUri_then_complex_object_properties_are_added()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromUriAttribute").Operation;

            //// Assert
            Assert.AreEqual("My comment.", operation.Summary);
            Assert.AreEqual("My remarks.", operation.Description);
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[0].Kind);
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[1].Kind);
            Assert.AreEqual("Foo", operation.ActualParameters[0].Name);
            Assert.AreEqual("bar", operation.ActualParameters[1].Name);
            Assert.AreEqual("My comment.", operation.ActualParameters[1].Description);
        }

        [TestMethod]
        public async Task When_parameter_is_complex_and_has_FromBody_then_it_is_a_body_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromBodyAttribute").Operation;
            var json = document.ToJson();

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Body, operation.ActualParameters[0].Kind);
            Assert.AreEqual("data", operation.ActualParameters[0].Name);
        }

        public class ComplexPathParameterController : ApiController
        {
            [HttpGet]
            [Route("Filter/{title}")]
            public string Filter([FromUri]ComplexGetRequest model)
            {
                return "foobar = " + model.Title + " => " + model.Bar;
            }

            public class ComplexGetRequest
            {
                [Required]
                [FromRoute]
                public string Title { get; set; }

                public string Bar { get; set; }
            }
        }

        [TestMethod]
        public async Task When_complex_parameter_contains_property_with_FromRoute_attribute_then_it_is_generated_as_path_parameter()
        {
            // FromRouteAttribute is only available in ASP.NET Core
            // Issue: https://github.com/NSwag/NSwag/issues/513

            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                IsAspNetCore = true, 
                DefaultPropertyNameHandling = PropertyNameHandling.CamelCase
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<ComplexPathParameterController>();
            var operation = document.Operations.First().Operation;
            var json = document.ToJson();

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Path, operation.Parameters[0].Kind);
            Assert.AreEqual("title", operation.Parameters[0].Name);
            Assert.AreEqual(SwaggerParameterKind.Query, operation.Parameters[1].Kind);
            Assert.AreEqual("bar", operation.Parameters[1].Name);
        }
    }
}

namespace Microsoft.AspNetCore.Mvc
{
    public class FromRouteAttribute : Attribute
    {
        public string Name { get; set; }
    }
}