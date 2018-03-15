using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;

namespace NSwag.CodeGeneration.Tests
{
    [TestClass]
    public class CodeGenerationTests
    {
        [TestMethod]
        public async Task When_generating_CSharp_code_then_output_contains_expected_classes()
        {
            // Arrange
            var document = await CreateDocumentAsync();
            var json = document.ToJson();

            //// Act
            var settings = new SwaggerToCSharpClientGeneratorSettings { ClassName = "MyClass" };
            settings.CSharpGeneratorSettings.Namespace = "MyNamespace";

            var generator = new SwaggerToCSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.IsTrue(code.Contains("namespace MyNamespace"));
            Assert.IsTrue(code.Contains("class MyClass"));
            Assert.IsTrue(code.Contains("class Person"));
            Assert.IsTrue(code.Contains("class Address"));
        }

        [TestMethod]
        public async Task When_generating_TypeScript_code_then_output_contains_expected_classes()
        {
            // Arrange
            var document = await CreateDocumentAsync();

            //// Act
            var generator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                ClassName = "MyClass",
                TypeScriptGeneratorSettings = 
                {
                    TypeStyle = TypeScriptTypeStyle.Interface
                }
            });
            var code = generator.GenerateFile();

            // Assert
            Assert.IsTrue(code.Contains("export class MyClass"));
            Assert.IsTrue(code.Contains("export interface Person"));
            Assert.IsTrue(code.Contains("export interface Address"));
        }

        [TestMethod]
        public async Task When_using_json_schema_with_references_in_service_then_references_are_correctly_resolved()
        {
            //// Arrange
            var jsonSchema = @"{
  ""definitions"": {
    ""app"": {
      ""definitions"": {
        ""name"": {
          ""pattern"": ""^[a-z][a-z0-9-]{3,30}$"",
          ""type"": ""string""
        }
      },
      ""properties"": {
        ""name"": {
          ""$ref"": ""#/definitions/app/definitions/name""
        }
      },
      ""required"": [""name""],
      ""type"": ""object""
    }
  },
  ""properties"": {
    ""app"": {
      ""$ref"": ""#/definitions/app""
    },
  },
  ""type"": ""object""
}";

            //// Act
            var schema = await JsonSchema4.FromJsonAsync(jsonSchema);
            var document = new SwaggerDocument();
            document.Definitions["Foo"] = schema;

            //// Assert
            var jsonService = document.ToJson(); // no exception expected
        }

        private static async Task<SwaggerDocument> CreateDocumentAsync()
        {
            var document = new SwaggerDocument();
            var settings = new JsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(settings);

            document.Paths["/Person"] = new SwaggerPathItem();
            document.Paths["/Person"][SwaggerOperationMethod.Get] = new SwaggerOperation
            {
                Responses = 
                {
                    {
                        "200", new SwaggerResponse
                        {
                            Schema = new JsonSchema4
                            {
                                SchemaReference = await generator.GenerateAsync(typeof(Person), new SwaggerSchemaResolver(document, settings))
                            }
                        }
                    }
                }
            };
            return document;
        }
    }

    public class Person
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public Sex Sex { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }

        public string City { get; set; }
    }

    public enum Sex
    {
        Male,
        Female
    }
}
