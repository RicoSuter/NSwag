using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;

namespace NSwag.CodeGeneration.Tests.WebApi.Attributes
{
    [TestClass]
    public class PrimitiveQueryParametersTests
    {
        public class TestController : ApiController
        {
            public string WithoutAttribute(string foo)
            {
                return String.Empty;
            }

            public string WithFromUriAttribute([FromUri] string foo)
            {
                return String.Empty;
            }

            public string WithFromBodyAttribute([FromBody] string foo)
            {
                return String.Empty;
            }

            public class FilterOptions
            {
                [JsonProperty("currentStates")]
                public string[] CurrentStates { get; set; }
            }

            public void Filter([FromUri] FilterOptions filter)
            {

            }
        }

        [TestMethod]
        public async Task When_parameter_is_primitive_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithoutAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[0].Kind);
        }

        [TestMethod]
        public async Task When_parameter_is_primitive_and_has_FromUri_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new SwaggerGenerators.WebApi.WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromUriAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Query, operation.ActualParameters[0].Kind);
        }


        [TestMethod]
        public async Task When_parameter_is_primitive_and_has_FromBody_then_it_is_a_body_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_WithFromBodyAttribute").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterKind.Body, operation.ActualParameters[0].Kind);
        }

        [TestMethod]
        public async Task When_FromUri_has_array_property_then_collectionFormat_is_set_to_multi()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<TestController>();
            var operation = document.Operations.Single(o => o.Operation.OperationId == "Test_Filter").Operation;

            //// Assert
            Assert.AreEqual(SwaggerParameterCollectionFormat.Multi, operation.ActualParameters[0].CollectionFormat);
        }

        public class ControllerWithArrayQueryParameter : ApiController
        {
            public string Foo([FromUri] string[] ids)
            {
                return string.Empty;
            }
        }

        [TestMethod]
        public async Task When_parameter_is_array_and_has_FromUri_then_it_is_a_query_parameter()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}"
            });

            //// Act
            var document = await generator.GenerateForControllerAsync<ControllerWithArrayQueryParameter>();
            var json = document.ToJson();

            //// Assert
            var operation = document.Operations.First().Operation;
            var parameter = operation.ActualParameters.First();

            Assert.AreEqual(SwaggerParameterKind.Query, parameter.Kind);
            Assert.AreEqual(JsonObjectType.Array, parameter.Type);
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
        public async Task When_query_parameter_is_enum_array_then_the_enum_is_referenced()
        {
            //// Arrange
            var settings = new WebApiToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = "api/{controller}/{action}/{id}",
                DefaultEnumHandling = EnumHandling.String,
                DefaultPropertyNameHandling = PropertyNameHandling.Default,
                NullHandling = NullHandling.Swagger,
            };
            var generator = new WebApiToSwaggerGenerator(settings);

            //// Act
            var document = await generator.GenerateForControllerAsync<FooController>();
            var json = document.ToJson();

            var gen = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings { Template = TypeScriptTemplate.JQueryCallbacks });
            var code = gen.GenerateFile();

            //// Assert
            Assert.IsNotNull(document.Operations.First().Operation.Parameters.First().Item.SchemaReference);
            Assert.IsTrue(code.Contains("getFoos(bars: Bar[], "));
        }

        public class MyTestClass : ApiController
        {
            [Route("Foo")]
            public async Task Foo(int id, bool? someNullableParam)
            {

            }

            [Route("Bar")]
            public async Task Bar(int id, bool? someNullableParam = null)
            {

            }
        }

        [TestMethod]
        public async Task When_parameter_is_nullable_then_it_is_optional()
        {
            //// Arrange
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings { IsAspNetCore = true });

            //// Act
            var document = await generator.GenerateForControllerAsync<MyTestClass>();
            var json = document.ToJson();

            //// Assert
            Assert.IsTrue(document.Operations.First().Operation.Parameters[1].IsRequired);
            Assert.IsFalse(document.Operations.Last().Operation.Parameters[1].IsRequired);
        }
    }
}
