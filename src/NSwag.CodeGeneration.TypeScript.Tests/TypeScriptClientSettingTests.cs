using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NJsonSchema.CodeGeneration.TypeScript;
using NSwag.Generation.WebApi;
using System.Runtime.Serialization;
using Xunit;
using NJsonSchema.NewtonsoftJson.Converters;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using static NSwag.CodeGeneration.TypeScript.Tests.OperationParameterTests;

namespace NSwag.CodeGeneration.TypeScript.Tests
{
    public class TypeScriptClientSettingTests
    {
        public class FooController
        {
            [Route("test")]
            public string Test(int a, int? b)
            {
                return null;
            }

            [Obsolete("Obsolete endpoint for testing")]
            [Route("obsoleteEndpoint")]
            public string ObsoleteEndpoint(int a, int? b)
            {
                return null;
            }
        }

        [Fact]
        public async Task When_depreacted_endpoints_are_excluded_the_client_will_not_generate_these_endpoint()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                ExcludeDeprecated = true
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.DoesNotContain("obsoleteEndpoint", code);
            Assert.DoesNotContain("deprecated", code);
            Assert.Contains("test", code); // contains other endpoint
        }

        [Fact]
        public async Task When_regex_is_set_to_excluded_endpoints_the_client_will_not_generate_these_endpoint()
        {
            // Arrange
            var swaggerGenerator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings()
            });

            var document = await swaggerGenerator.GenerateForControllerAsync<FooController>();
            var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
            {
                ExcludeByPathRegex = "test"
            });

            // Act
            var code = generator.GenerateFile();

            // Assert
            Assert.DoesNotContain("foo", code);
            Assert.Contains("obsoleteEndpoint", code); // contains other endpoint
            Assert.Contains("deprecated", code);
        }
    }
}
