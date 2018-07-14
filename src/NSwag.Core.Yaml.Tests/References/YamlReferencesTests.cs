using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.References;
using Xunit;

namespace NSwag.Core.Yaml.Tests.References
{
    public class YamlReferencesTests
    {

        [Theory]
        [InlineData("/References/YamlReferencesTest/json_contract_with_json_reference.json", "./baseContract.json#/definitions/myInt")]
        [InlineData("/References/YamlReferencesTest/yaml_contract_with_json_reference.yaml", "./baseContract.json#/definitions/myInt")]
        [InlineData("/References/YamlReferencesTest/yaml_contract_with_yaml_reference.yaml", "./baseContract.yaml#/definitions/myInt")]
        [InlineData("/References/YamlReferencesTest/json_contract_with_yaml_reference.json", "./baseContract.yaml#/definitions/myInt")]
        public async Task When_yaml_schema_has_references_it_works(string relativePath, string documentPath)
        {
            //// Arrange
            var path = GetTestDirectory() + relativePath;

            //// Act
            var document = await SwaggerYamlDocument.FromFileAsync(path);
            var json = document.ToJson();

            //// Assert
            Assert.Equal(JsonObjectType.Integer, document.Definitions["ContractObject"].Properties["foo"].ActualTypeSchema.Type);
            Assert.Equal(JsonObjectType.Boolean, document.Definitions["ContractObject"].Properties["bar"].ActualTypeSchema.Type);
        }

        private string GetTestDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
    }
}