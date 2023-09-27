using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses;
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
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
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
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(ResponsesController));

            // Assert
            var operation = document.Operations.First().Operation;

            Assert.True(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }

        [Fact]
        public async Task When_nullable_xml_docs_is_set_to_true_then_response_is_nullable()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(NullableResponseController));

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(NullableResponseController.OperationWithNullableResponse))).Operation;

            Assert.True(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }

        [Fact]
        public async Task When_nullable_xml_docs_is_set_to_false_then_response_is_not_nullable()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(NullableResponseController));

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(NullableResponseController.OperationWithNonNullableResponse))).Operation;

            Assert.False(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }

        [Fact]
        public async Task When_nullable_xml_docs_is_not_set_then_default_setting_NotNull_is_used()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(NullableResponseController));

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(NullableResponseController.OperationWithNoXmlDocs))).Operation;

            Assert.False(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }

        [Fact]
        public async Task When_nullable_xml_docs_is_not_set_then_default_setting_Null_is_used()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(NullableResponseController));

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(NullableResponseController.OperationWithNoXmlDocs))).Operation;

            Assert.True(operation.ActualResponses.First().Value.Schema.IsNullable(SchemaType.OpenApi3));
        }
    }
}