using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.ClientGenerators.CSharp;
using NSwag.CodeGeneration.ClientGenerators.TypeScript;

namespace NSwag.CodeGeneration.Tests
{
    [TestClass]
    public class CodeGenerationTests
    {
        [TestMethod]
        public void When_generating_CSharp_code_then_output_contains_expected_classes()
        {
            // Arrange
            var service = CreateService();

            //// Act
            var generator = new SwaggerToCSharpGenerator(service);
            generator.Class = "MyClass";
            generator.Namespace = "MyNamespace";
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
            var service = CreateService();

            //// Act
            var generator = new SwaggerToTypeScriptGenerator(service);
            generator.Class = "MyClass";
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
            var service = new SwaggerService();
            service.Definitions["Foo"] = schema;

            //// Assert
            var jsonService = service.ToJson(); // no exception expected
        }

        private static SwaggerService CreateService()
        {
            var service = new SwaggerService();
            service.Paths["/Person"] = new SwaggerOperations();
            service.Paths["/Person"][SwaggerOperationMethod.Get] = new SwaggerOperation
            {
                Responses = new Dictionary<string, SwaggerResponse>
                {
                    {
                        "200", new SwaggerResponse
                        {
                            Schema = JsonSchema4.FromType(typeof (Person))
                        }
                    }
                }
            };
            return service;
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
