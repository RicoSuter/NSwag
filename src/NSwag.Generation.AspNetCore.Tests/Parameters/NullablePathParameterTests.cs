using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class NonNullablePathParameterTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_NRT_is_enabled_and_string_parameter_is_not_nullable_then_parameter_should_not_be_nullable()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaType = SchemaType.OpenApi3
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(NonNullablePathParameterControl));
            var json = document.ToJson();

            // Assert
            var nonNullableParameter = document.Operations.First().Operation.Parameters.First();
            var nullableParameter = document.Operations.First().Operation.Parameters.Last();

            Assert.False(nonNullableParameter.IsNullable(SchemaType.OpenApi3));
            Assert.True(nullableParameter.IsNullable(SchemaType.OpenApi3));
        }
    }
}
