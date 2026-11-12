using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;

namespace NSwag.CodeGeneration.CSharp.Tests
{
    public class ModelNamePrefixSuffixTests
    {
        private static OpenApiDocument CreateDocument()
        {
            var document = new OpenApiDocument();
            var settings = new NewtonsoftJsonSchemaGeneratorSettings();
            var generator = new JsonSchemaGenerator(settings);

            document.Paths["/person"] = new OpenApiPathItem
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
                                    Reference = generator.Generate(typeof(SamplePerson), new OpenApiSchemaResolver(document, settings))
                                }
                            }
                        }
                    }
                }
            };

            return document;
        }

        [Fact]
        public void When_ModelNameSuffix_is_set_then_generated_model_class_has_suffix()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNameSuffix = "Dto"
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("class SamplePersonDto", code);
            Assert.DoesNotContain("class SamplePerson ", code);
        }

        [Fact]
        public void When_ModelNamePrefix_is_set_then_generated_model_class_has_prefix()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNamePrefix = "My"
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("class MySamplePerson", code);
            Assert.DoesNotContain("class SamplePerson ", code);
        }

        [Fact]
        public void When_both_ModelNamePrefix_and_ModelNameSuffix_are_set_then_generated_model_class_has_both()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNamePrefix = "My",
                ModelNameSuffix = "Dto"
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("class MySamplePersonDto", code);
            Assert.DoesNotContain("class SamplePerson ", code);
        }

        [Fact]
        public void When_ModelNamePrefix_and_ModelNameSuffix_are_empty_then_generated_model_class_name_is_unchanged()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNamePrefix = "",
                ModelNameSuffix = ""
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("class SamplePerson", code);
        }

        [Fact]
        public void When_ModelNameSuffix_is_set_then_return_type_in_client_also_uses_suffix()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNameSuffix = "Dto"
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert
            Assert.Contains("SamplePersonDto", code);
            Assert.DoesNotContain("Task<SamplePerson>", code);
        }
        [Fact]
        public void When_ModelNameSuffix_is_set_then_enum_names_are_not_affected()
        {
            // Arrange
            var document = CreateDocument();
            var settings = new CSharpClientGeneratorSettings
            {
                ModelNameSuffix = "Dto"
            };

            // Act
            var generator = new CSharpClientGenerator(document, settings);
            var code = generator.GenerateFile();

            // Assert - enum is unchanged
            Assert.Contains("enum SampleStatus", code);
            Assert.DoesNotContain("enum SampleStatusDto", code);
            // class still gets the suffix
            Assert.Contains("class SamplePersonDto", code);
        }
    }

    public class SamplePerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public SampleStatus Status { get; set; }
    }

    public enum SampleStatus
    {
        Active,
        Inactive
    }
}
