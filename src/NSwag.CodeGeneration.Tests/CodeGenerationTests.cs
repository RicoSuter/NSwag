using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NSwag.CodeGeneration.CodeGenerators.CSharp;
using NSwag.CodeGeneration.CodeGenerators.TypeScript;

namespace NSwag.CodeGeneration.Tests
{
    [TestClass]
    public class CodeGenerationTests
    {
        [TestMethod]
        public void When_generating_CSharp_code_then_output_contains_expected_classes()
        {
            // Arrange
            var document = CreateService();
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
        public void When_generating_TypeScript_code_then_output_contains_expected_classes()
        {
            // Arrange
            var document = CreateService();

            //// Act
            var generator = new SwaggerToTypeScriptClientGenerator(document, new SwaggerToTypeScriptClientGeneratorSettings
            {
                ClassName = "MyClass",
                TypeScriptGeneratorSettings = new TypeScriptGeneratorSettings
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
        public void When_using_json_schema_with_references_in_service_then_references_are_correctly_resolved()
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
            var schema = JsonSchema4.FromJson(jsonSchema);
            var document = new SwaggerDocument();
            document.Definitions["Foo"] = schema;

            //// Assert
            var jsonService = document.ToJson(); // no exception expected
        }

        private static SwaggerDocument CreateService()
        {
            var document = new SwaggerDocument();
            var settings = new JsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(settings);

            document.Paths["/Person"] = new SwaggerOperations();
            document.Paths["/Person"][SwaggerOperationMethod.Get] = new SwaggerOperation
            {
                Responses = new Dictionary<string, SwaggerResponse>
                {
                    {
                        "200", new SwaggerResponse
                        {
                            Schema = new JsonSchema4
                            {
                                SchemaReference = generator.Generate(typeof(Person), new SwaggerSchemaResolver(document, settings))
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
