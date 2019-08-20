using System.Linq;
using System.Threading.Tasks;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests
{
    public class CallbackTests : AspNetCoreTestsBase
    {       
        [Fact]
        public async Task When_operation_has_extension_data_attributes_then_they_are_processed()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(SwaggerCallbackController));

            //// Assert
            var operation = document.Operations.First().Operation;
            var callbacks = operation.Callbacks;

            /*
                [OpenApiCallback("url1")]
                [OpenApiCallback("url2", "emptyGet", "get")]
                [OpenApiCallback("url3", "single", null, typeof(string))]
                [OpenApiCallback("url4", "multiple", null, typeof(string), typeof(MyPayload))]        
             */

            Assert.Equal(4, callbacks.Count);

            Assert.Equal("url1", callbacks[operation.OperationId + "_Callback"].First().Key);
            Assert.Equal("post", callbacks[operation.OperationId + "_Callback"].First().Value.First().Key);
            Assert.Null(callbacks[operation.OperationId + "_Callback"].First().Value.First().Value.RequestBody);

            Assert.Equal("url2", callbacks["emptyGet"].First().Key);
            Assert.Equal("get", callbacks["emptyGet"].First().Value.First().Key);
            Assert.Null(callbacks["single"].First().Value.First().Value.RequestBody);
            
            Assert.Equal("url3", callbacks["single"].First().Key);
            Assert.Equal("post", callbacks["single"].First().Value.First().Key);
            Assert.Equal(NJsonSchema.JsonObjectType.String, callbacks["single"].First().Value.First().Value.RequestBody.Content["application/json"].Schema.Type);

            Assert.Equal("url4", callbacks["multiple"].First().Key);
            Assert.Equal("post", callbacks["multiple"].First().Value.First().Key);
            var schema = callbacks["multiple"].First().Value.First().Value.RequestBody.Content["application/json"].Schema;
            Assert.Equal(NJsonSchema.JsonObjectType.Object, schema.Type);
            Assert.Equal(NJsonSchema.JsonObjectType.String, schema.OneOf.First().Type);
            Assert.Equal(NJsonSchema.JsonObjectType.Object, schema.OneOf.Last().Type);
            Assert.Equal("myProperty", schema.OneOf.Last().Properties.First().Key);
            Assert.Equal(NJsonSchema.JsonObjectType.Integer, schema.OneOf.Last().Properties["myProperty"].Type);

        }
    }
}
