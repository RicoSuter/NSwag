using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Annotations;

namespace NSwag.Generation.WebApi.Tests.Nullability
{
    [TestClass]
    public class ParameterNullabilityTests
    {
        public class NotNullParameterTestController : Controller
        {
            [Route("Abc")]
            public object Abc([NotNull]object foo, object bar)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_parameter_has_NotNullAttribute_then_it_is_not_nullable()
        {
            //// Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings());

            //// Act
            var document = await generator.GenerateForControllerAsync<NotNullParameterTestController>();
            var json = document.ToJson();

            //// Assert 
            Assert.IsFalse(document.Operations.First().Operation.Parameters[0].IsNullable(SchemaType.Swagger2));
            Assert.IsTrue(document.Operations.First().Operation.Parameters[1].IsNullable(SchemaType.Swagger2));
        }

        public class NullableSchemaTestController : Controller
        {

            public enum MyEnum
            {
                Foo = 1,
                Bar = 20
            }

            [Route("Abc")]
            [HttpGet]
            public object Abc(
                string foo, 
                MyEnum? bar = null, 
                List<MyEnum?> manyBars = null
            )
            {
                return null;
            }
        }

        [TestMethod]
        public async Task Get_NullableQueryParamEnums_ShouldHaveSingleOneOf()
        {
            //// Arrange
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings() { SchemaType = SchemaType.OpenApi3, AllowNullableBodyParameters = false });

            //// Act
            var document = await generator.GenerateForControllerAsync<NullableSchemaTestController>();

            document.Generator = null; // Remove version string for easier assertion

            var json = document.ToJson(SchemaType.OpenApi3, Formatting.None);

            //// Assert
            // Verify there is no double OneOf - OneOf schema on second parameter
            // Verify there is no requestBody on GET operation
            Assert.AreEqual("{\"openapi\":\"3.0.0\",\"info\":{\"title\":\"My Title\",\"version\":\"1.0.0\"},\"paths\":{\"/Abc\":{\"get\":{\"tags\":[\"NullableSchemaTest\"],\"operationId\":\"NullableSchemaTest_Abc\",\"parameters\":[{\"name\":\"foo\",\"in\":\"query\",\"required\":true,\"schema\":{\"type\":\"string\",\"nullable\":true},\"x-position\":1},{\"name\":\"bar\",\"in\":\"query\",\"schema\":{\"oneOf\":[{\"nullable\":true,\"$ref\":\"#/components/schemas/MyEnum\"}]},\"x-position\":2},{\"name\":\"manyBars\",\"in\":\"query\",\"style\":\"form\",\"explode\":true,\"schema\":{\"type\":\"array\",\"nullable\":true,\"items\":{\"nullable\":true,\"oneOf\":[{\"$ref\":\"#/components/schemas/MyEnum\"}]}},\"x-position\":3}],\"responses\":{\"200\":{\"description\":\"\",\"content\":{\"application/json\":{\"schema\":{\"nullable\":true}}}}}}}},\"components\":{\"schemas\":{\"MyEnum\":{\"type\":\"integer\",\"description\":\"\",\"x-enumNames\":[\"Foo\",\"Bar\"],\"enum\":[1,20]}}}}", json);
        }
    }
}