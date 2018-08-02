using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class LanguageParameterTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_implicit_language_parameter_is_used_then_it_is_in_spec()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings);
            var json = document.ToJson();

            // Assert
            var operation = GetControllerOperations(document, "Languages").Single();

            Assert.Equal(2, operation.Operation.ActualParameters.Count);
            Assert.Contains(operation.Operation.ActualParameters, p => p.Name == "language");

            var parameter = operation.Operation.ActualParameters.Single(p => p.Name == "language");
            Assert.Equal(JsonObjectType.String, parameter.ActualTypeSchema.Type);
            Assert.False(parameter.IsNullable(SchemaType.Swagger2));
        }
    }
}
