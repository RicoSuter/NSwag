using Xunit;
using NJsonSchema;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses;
using NJsonSchema.NewtonsoftJson.Generation;

namespace NSwag.Generation.AspNetCore.Tests.Responses
{
    public class WrappedResponseTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_response_is_wrapped_in_certain_generic_result_types_then_discard_the_wrapper_type()
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
            var document = await GenerateDocumentAsync(settings, typeof(WrappedResponseController));

            // Assert
            OpenApiResponse GetOperationResponse(string actionName)
            {
                return document.Operations
                    .Single(op => op.Operation.OperationId == $"{nameof(WrappedResponseController)
                        .Substring(0, nameof(WrappedResponseController).Length - "Controller".Length)}_{actionName}").Operation.ActualResponses.Single().Value;
            }

            JsonObjectType GetOperationResponseSchemaType(string actionName)
            {
                return GetOperationResponse(actionName).Schema.Type;
            }

            var intType = NewtonsoftJsonSchemaGenerator.FromType<int>().Type;

            Assert.Null(GetOperationResponse(nameof(WrappedResponseController.Task)).Schema);
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.Int)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.TaskOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.ValueTaskOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.ActionResultOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.TaskOfActionResultOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof(WrappedResponseController.ValueTaskOfActionResultOfInt)));
        }
    }
}