using System.ComponentModel.DataAnnotations;
using NJsonSchema;
using NJsonSchema.CodeGeneration.TypeScript;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.CodeGeneration.Tests;

namespace NSwag.CodeGeneration.TypeScript.Tests;

public class CodeGenerationTests
{
    [Fact]
    public async Task When_generating_TypeScript_code_then_output_contains_expected_classes()
    {
        // Arrange
        var document = CreateDocument();

        // Act
        var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
        {
            ClassName = "MyClass",
            TypeScriptGeneratorSettings =
            {
                TypeStyle = TypeScriptTypeStyle.Interface
            }
        });
        var code = generator.GenerateFile();

        // Assert
        await VerifyHelper.Verify(code);
        TypeScriptCompiler.AssertCompile(code);

    }

    private static OpenApiDocument CreateDocument()
    {
        var document = new OpenApiDocument();
        var settings = new NewtonsoftJsonSchemaGeneratorSettings();
        var generator = new JsonSchemaGenerator(settings);

        document.Paths["/Person"] = new OpenApiPathItem
        {
            [OpenApiOperationMethod.Get] = new OpenApiOperation
            {
                Responses =
                {
                    {
                        "200", new OpenApiResponse
                        {
                            Schema = new JsonSchema
                            {
                                Reference = generator.Generate(typeof(Person), new OpenApiSchemaResolver(document, settings))
                            }
                        }
                    }
                }
            }
        };
        return document;
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