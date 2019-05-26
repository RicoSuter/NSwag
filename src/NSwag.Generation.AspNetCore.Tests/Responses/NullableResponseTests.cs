using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests.Responses
{
    public class NullableResponseTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_handling_is_NotNull_then_response_is_not_nullable()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaType = SchemaType.OpenApi3,
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ResponsesController));

            // Assert
            var operation = document.Operations.First().Operation;

            Assert.False(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }

        [Fact]
        public async Task When_handling_is_Null_then_response_is_nullable()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaType = SchemaType.OpenApi3,
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ResponsesController));

            // Assert
            var operation = document.Operations.First().Operation;

            Assert.True(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }
    }
}