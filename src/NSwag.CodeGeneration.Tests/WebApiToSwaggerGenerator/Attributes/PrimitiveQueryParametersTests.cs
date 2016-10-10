using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApiToSwaggerGenerator.Attributes
{
    [TestClass]
    public class PrimitiveQueryParametersTests
    {
        public class TestController : ApiController
        {
            public string WithoutAttribute(string foo)
            {
                return string.Empty;
            }

            public string WithFromUriAttribute([FromUri] string foo)
            {
                return string.Empty;
            }

            public string WithFromBodyAttribute([FromBody] string foo)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public void When_parameter_is_primitive_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "Test_WithoutAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[0].Kind);
        }

        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromUri_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "Test_WithFromUriAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[0].Kind);
        }


        [TestMethod]
        public void When_parameter_is_primitive_and_has_FromBody_then_it_is_a_body_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<TestController>();
            var operation = service.Operations.Single(o => o.Operation.OperationId == "Test_WithFromBodyAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Body, operation.ActualParameters[0].Kind);
        }

        public class ControllerWithArrayQueryParameter : ApiController
        {
            public string Foo([FromUri] string[] ids)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public void When_parameter_is_array_and_has_FromUri_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var service = generator.GenerateForController<ControllerWithArrayQueryParameter>();
            var json = service.ToJson();

            //// Assert
            var operation = service.Operations.First().Operation;
            var parameter = operation.ActualParameters.First();

            Assert.AreEqual(SwaggerParameterKind.Query, parameter.Kind);
            Assert.AreEqual(JsonObjectType.String, parameter.Type);
            Assert.AreEqual(SwaggerParameterCollectionFormat.Multi, parameter.CollectionFormat);
        }

        public class FooController : ApiController
        {
            [Route("foos/")]
            public Foo[] GetFoos([FromUri] Bar[] bars)
            {
                return new Foo[0];
            }
        }

        public enum Bar
        {
            Baz,
            Foo
        }

        public class Foo
        {
            public Bar Bar { get; set; }

            public Bar Bar2 { get; set; }
        }

        [TestMethod]
        public void When_query_parameter_is_enum_array_then_the_enum_is_referenced()
        {
            //// Arrange
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}",
                DefaultEnumHandling = EnumHandling.String,
                DefaultPropertyNameHandling = PropertyNameHandling.Default,
                NullHandling = NullHandling.Swagger
            };
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(settings);

            //// Act
            var service = generator.GenerateForController<FooController>();
            var json = service.ToJson();

            //// Assert
            Assert.IsNotNull(service.Operations.First().Operation.Parameters.First().Schema.SchemaReference);
        }
    }
}
