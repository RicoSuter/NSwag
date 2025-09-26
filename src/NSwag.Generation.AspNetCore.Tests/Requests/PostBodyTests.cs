using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Requests;

namespace NSwag.Generation.AspNetCore.Tests.Requests
{
    public class PostBodyTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_OpenApiBodyParameter_is_applied_with_JSON_then_request_body_is_any_type()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(PostBodyController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "PostBody_JsonPostBodyOperation").Operation;
            var parameter = operation.Parameters.Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.Single(operation.Parameters);
            Assert.True(operation.RequestBody.Content["application/json"].Schema.IsAnyType);
        }

        [Fact]
        public async Task When_OpenApiBodyParameter_is_applied_with_text_then_request_body_is_file()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(PostBodyController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Operation.OperationId == "PostBody_FilePostBodyOperation").Operation;
            var parameter = operation.Parameters.Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.Single(operation.Parameters);
            Assert.Equal(JsonObjectType.String, operation.RequestBody.Content["text/plain"].Schema.Type);
            Assert.Equal("binary", operation.RequestBody.Content["text/plain"].Schema.Format);
        }
    }
}