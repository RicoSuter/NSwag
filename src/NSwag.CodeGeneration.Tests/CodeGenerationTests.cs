using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.OperationNameGenerators;
using NSwag.CodeGeneration.TypeScript;
using Xunit;

namespace NSwag.CodeGeneration.Tests
{
    public class CodeGenerationTests
    {
        [Fact]
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
            Assert.Contains("namespace MyNamespace", code);
            Assert.Contains("class MyClass", code);
            Assert.Contains("class Person", code);
            Assert.Contains("class Address", code);
        }

        [Fact]
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
            Assert.Contains("export class MyClass", code);
            Assert.Contains("export interface Person", code);
            Assert.Contains("export interface Address", code);
        }

        [Fact]
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

        [Fact]
        public void No_Brackets_in_Operation_Name() 
        {
            // Arrange
            var path = "/my/path/with/{parameter_with_underscore}/and/{another_parameter}";

            //// Act
            var operationName = SingleClientFromPathSegmentsOperationNameGenerator.ConvertPathToName(path);

            // Assert
            Assert.DoesNotContain("{", operationName);
            Assert.DoesNotContain("}", operationName);
            Assert.False(string.IsNullOrWhiteSpace(operationName));
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
                                Reference = await generator.GenerateAsync(typeof(Person), new SwaggerSchemaResolver(document, settings))
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
