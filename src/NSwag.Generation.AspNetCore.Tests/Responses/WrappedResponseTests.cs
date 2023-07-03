using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using NJsonSchema;

using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Responses;

namespace NSwag.Generation.AspNetCore.Tests.Responses
{
    public class WrappedResponseTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_response_is_wrapped_in_certain_generic_result_types_then_discard_the_wrapper_type()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(WrappedResponseController));

            // Assert
            OpenApiResponse GetOperationResponse(string actionName) => document.Operations
                .Where(op => op.Operation.OperationId == $"{nameof(WrappedResponseController)
                    .Substring(0, nameof(WrappedResponseController).Length - "Controller".Length)}_{actionName}")
                .Single().Operation.ActualResponses.Single().Value;

            JsonObjectType GetOperationResponseSchemaType(string actionName) =>
                GetOperationResponse(actionName).Schema.Type;

            var intType = JsonSchema.FromType<int>().Type;

            Assert.Null(GetOperationResponse(nameof(WrappedResponseController.Task)).Schema);
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.Int)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.TaskOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.ValueTaskOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.ActionResultOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.TaskOfActionResultOfInt)));
            Assert.Equal(intType, GetOperationResponseSchemaType(nameof( WrappedResponseController.ValueTaskOfActionResultOfInt)));
        }
    }
}