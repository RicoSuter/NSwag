using System.Text.Json.Nodes;
using NJsonSchema;
using Xunit;

namespace NSwag.Core.Yaml.Tests
{
    public class YamlDocumentTests
    {
        [Fact]
        public async Task When_yaml_with_description_is_loaded_then_document_is_not_null()
        {
            // Arrange
            var yaml = @"info:
  title: Foo
  version: 1.0.0
paths:
  /something:
    description: foo
    get:
      responses:
        200:
          description: get description";

            // Act
            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);
            yaml = document.ToYaml();

            // Assert
            Assert.NotNull(document);
            Assert.Equal("foo", document.Paths.First().Value.Description);
            Assert.Contains("description: foo", yaml);
        }

        [Fact]
        public async Task When_yaml_with_custom_property_is_loaded_then_document_is_not_null()
        {
            // Arrange
            var yaml = @"swagger: '2.0'
info:
  title: foo
  version: '1.0'
paths:
  /bar:
    x-swagger-router-controller: bar
    get:
      responses:
        '200':
          description: baz";

            // Act
            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);
            yaml = document.ToYaml();

            // Assert
            Assert.NotNull(document);
            Assert.Equal("bar", document.Paths.First().Value.ExtensionData["x-swagger-router-controller"]?.ToString());
            Assert.Contains("x-swagger-router-controller: bar", yaml);
        }

        [Fact]
        public async Task When_yaml_with_custom_property_which_is_an_object_is_loaded_then_document_is_not_null()
        {
            // Arrange
            var yaml = @"swagger: '2.0'
info:
  title: foo
  version: '1.0'
paths:
  /bar:
    x-swagger-router-controller:
      bar: baz
    get:
      responses:
        '200':
          description: baz";

            // Act
            var document = await OpenApiYamlDocument.FromYamlAsync(yaml);
            yaml = document.ToYaml();

            // Assert
            Assert.NotNull(document);
            var extensionData = document.Paths.First().Value.ExtensionData["x-swagger-router-controller"];
            var jsonObj = extensionData is JsonObject jo ? jo : JsonNode.Parse(extensionData.ToString())?.AsObject();
            Assert.Equal("baz", jsonObj["bar"]?.GetValue<string>());
            Assert.Equal("baz", document.Paths.First().Value["get"].Responses["200"].Description);
            Assert.Contains("bar: baz", yaml);
        }

        [Fact]
        public async Task When_yaml_openapi3_is_loaded_then_roundtrip_preserves_structure()
        {
            // Arrange
            var yaml = @"openapi: 3.0.0
info:
  title: Test API
  version: 1.0.0
paths:
  /items:
    get:
      operationId: listItems
      parameters:
      - name: id
        in: path
        required: true
        schema:
          type: string
      - name: filter
        in: query
        schema:
          type: string
      responses:
        '200':
          description: A list of items";

            // Act
            var document = await OpenApiYamlDocument.FromYamlAsync(yaml, null, SchemaType.OpenApi3);
            var roundTrippedYaml = document.ToYaml();
            var document2 = await OpenApiYamlDocument.FromYamlAsync(roundTrippedYaml, null, SchemaType.OpenApi3);

            // Assert
            Assert.Equal("Test API", document2.Info.Title);
            Assert.Equal("1.0.0", document2.Info.Version);

            var parameters = document2.Paths["/items"]["get"].Parameters;
            Assert.Equal(2, parameters.Count);
            Assert.True(parameters.First(p => p.Name == "id").IsRequired);
            Assert.False(parameters.First(p => p.Name == "filter").IsRequired);

            Assert.Contains("operationId: listItems", roundTrippedYaml);
            Assert.Contains("required: true", roundTrippedYaml);
        }
    }
}