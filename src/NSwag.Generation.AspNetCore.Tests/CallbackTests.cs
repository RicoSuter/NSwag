using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers;
using NSwag.Generation.Processors;
using Xunit;

namespace NSwag.Generation.AspNetCore.Tests
{
    public class CallbackTests : AspNetCoreTestsBase
    {       
        [Fact]
        public async Task When_operation_has_callback_attributes_then_they_are_processed()
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

        /// <summary>
        /// Custom rule for callback responses: user must answer with 200 or 400 and write the callback name in the body
        /// </summary>
        private class CustomExpectedResponseOperationCallbackProcessor : OperationCallbackProcessor
        {
            public override IDictionary<string, OpenApiResponse> ExpectedResponses(string callbackName)
            {
                return new Dictionary<string, OpenApiResponse> {
                    {
                        "200", new OpenApiResponse
                        {
                            Description = $"Callback {callbackName} was accepted."
                        }
                    },{
                        "400", new OpenApiResponse
                        {
                            Description = $"Callback {callbackName} was rejected."
                        }
                    }
                };
            }
        }


    [Fact]
        public async Task When_callback_expected_responses_are_overridden_they_are_processed()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings();
            var customProcessor = new CustomExpectedResponseOperationCallbackProcessor();
            settings.OperationProcessors.Add(customProcessor);

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(SwaggerCallbackController));

            //// Assert
            var operation = document.Operations.First().Operation;
            var callbacks = operation.Callbacks;
            Func<string, IDictionary<string, OpenApiResponse>> getResponses = (name) => callbacks[name].First().Value.First().Value.Responses;

            /*
                [OpenApiCallback("url1")]
                [OpenApiCallback("url2", "emptyGet", "get")]
                [OpenApiCallback("url3", "single", null, typeof(string))]
                [OpenApiCallback("url4", "multiple", null, typeof(string), typeof(MyPayload))]        
            */

            foreach (var callbackName in new [] { operation.OperationId + "_Callback" , "emptyGet", "single", "multiple"})
            {
                var definedResponses = customProcessor.ExpectedResponses(callbackName);
                var generatedResponses = getResponses(callbackName);
                Assert.Equal(definedResponses.Count, generatedResponses.Count);
                Assert.All(definedResponses, dr => Assert.Equal(dr.Value.Description, generatedResponses[dr.Key].Description));
            }

        }
    }
}
