using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NJsonSchema;
using NSwag.CodeGeneration.ClientGenerators.CSharp;

namespace NSwag.CodeGeneration.Tests
{
    [TestClass]
    public class SwaggerServiceTests
    {
        [TestMethod]
        public void When_generating_code_then_output_is_not_null()
        {
            // Arrange
            var service = new SwaggerService();
            service.Paths["/Person"] = new SwaggerOperations();
            service.Paths["/Person"][SwaggerOperationMethod.get] = new SwaggerOperation
            {
                Responses = new Dictionary<string, SwaggerResponse>
                {
                    {
                        "200", new SwaggerResponse
                        {
                            Schema = JsonSchema4.FromType(typeof(Person))
                        }
                    }
                }
            };

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
