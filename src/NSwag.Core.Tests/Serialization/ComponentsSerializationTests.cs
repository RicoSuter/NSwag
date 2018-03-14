using System.Threading.Tasks;
using NJsonSchema;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class ComponentsSerializationTests
    {
        [Fact]
        public async Task When_schema_is_added_to_definitions_then_it_is_serialized_correctly_in_Swagger()
        {
            //// Arrange
            var document = CreateDocument();

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);
            document = await SwaggerDocument.FromJsonAsync(json);

            //// Assert
            Assert.Contains(@"""swagger""", json);
            Assert.DoesNotContain(@"""openapi""", json);

            Assert.Contains("definitions", json);
            Assert.DoesNotContain("components", json);

            Assert.True(document.Definitions.ContainsKey("Foo"));
        }

        [Fact]
        public async Task When_schema_is_added_to_definitions_then_it_is_serialized_correctly_in_OpenApi()
        {
            //// Arrange
            var document = CreateDocument();

            //// Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await SwaggerDocument.FromJsonAsync(json);

            //// Assert
            Assert.DoesNotContain(@"""swagger""", json);
            Assert.Contains(@"""openapi""", json);

            Assert.Contains("components", json);
            Assert.Contains("schemas", json);
            Assert.DoesNotContain("#/definitions/Foo", json);
            Assert.DoesNotContain("definitions", json);

            Assert.True(document.Definitions.ContainsKey("Foo"));
        }

        private static SwaggerDocument CreateDocument()
        {
            var schema = new JsonSchema4
            {
                Type = JsonObjectType.String
            };

            var document = new SwaggerDocument();
            document.Definitions["Foo"] = schema;
            document.Definitions["Bar"] = new JsonSchema4
            {
                Type = JsonObjectType.Object,
                Properties =
                {
                    { "Foo", new JsonProperty { Reference = schema } }
                }
            };

            return document;
        }
    }
}