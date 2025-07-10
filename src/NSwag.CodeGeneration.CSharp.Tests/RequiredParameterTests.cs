using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;

using NSwag.Generation.WebApi;

namespace NSwag.CodeGeneration.CSharp.Tests
{


    public abstract class UseRequiredKeywordTests<TSchemaGenerator>
            where TSchemaGenerator : JsonSchemaGeneratorSettings, new()
    {
        public class TestController : Controller
        {

            [Route("TestWithInput")]
            public void TestWithInput([FromBody] DTOWithRequiredFields input) { }
        }


        public class DTOWithRequiredFields
        {
            [Required]
            public int RequiredByAttribute { get; set; }

            public required string RequiredByC11Keyword { get; set; }

            [Required]
            public required string RequiredByAttributeAndC11Keyword { get; set; }

            public string NotRequired { get; set; }
        }

        private static async Task<string> GenerateCode(bool UseRequiredKeyword)
        {
            var generator = new WebApiOpenApiDocumentGenerator(new WebApiOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new TSchemaGenerator
                {

                },
            });

            var document = await generator.GenerateForControllerAsync<TestController>();

            // Act

            var codeGenerator = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings());
            codeGenerator.Settings.CSharpGeneratorSettings.UseRequiredKeyword = UseRequiredKeyword;

            return codeGenerator.GenerateFile();
        }

        [Fact]
        public async Task When_setting_is_enabled_properties_with_required_attribute_should_generate_with_required_keyword()
        {
            // Act
            var code = await GenerateCode(true);

            // Assert
            Assert.Contains("public required int RequiredByAttribute { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }

        [Fact (Skip = "The C#11 required keyword does not mark schema properties as required")]
        public async Task When_setting_is_enabled_properties_with_required_keyword_should_generate_with_required_keyword()
        {

            // Act
            var code = await GenerateCode(true);

            // Assert
            Assert.Contains("public required string RequiredByC11Keyword { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }

        [Fact]
        public async Task When_setting_is_enabled_properties_with_required_keyword_and_attribute_should_generate_with_required_keyword()
        {

            // Act
            var code = await GenerateCode(true);

            // Assert
            Assert.Contains("public required string RequiredByAttributeAndC11Keyword { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }


        [Fact]
        public async Task When_setting_is_disabled_properties_with_required_attribute_should_generate_with_required_keyword()
        {
            // Act
            var code = await GenerateCode(false);

            // Assert
            Assert.Contains("public int RequiredByAttribute { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }

        [Fact]
        public async Task When_setting_is_disabled_properties_with_required_keyword_should_generate_with_required_keyword()
        {

            // Act
            var code = await GenerateCode(false);

            // Assert
            Assert.Contains("public string RequiredByC11Keyword { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }

        [Fact]
        public async Task When_setting_is_disabled_properties_with_required_keyword_and_attribute_should_generate_with_required_keyword()
        {

            // Act
            var code = await GenerateCode(false);

            // Assert
            Assert.Contains("public string RequiredByAttributeAndC11Keyword { get; set; }", code);
            Assert.Contains("public string NotRequired { get; set; }", code);
        }
    }

    public class UseRequiredKeywordNewtonsoftJsonSchemaGeneratorTests : UseRequiredKeywordTests<NewtonsoftJsonSchemaGeneratorSettings>{ }
    public class UseRequiredKeywordSystemTextJsonSchemaGeneratorSettingsTests : UseRequiredKeywordTests<SystemTextJsonSchemaGeneratorSettings> { }
}
