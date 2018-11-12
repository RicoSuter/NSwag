//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessorTest.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NJsonSchema.Generation;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Processors.Tests
{
    public class OperationResponseProcessorTest
    {
        [Fact]
        public async Task ProcessAsync_AddsResponseFor200StatusCode()
        {
            // Arrange
            var apiDescription = new ApiDescription
            {
                SupportedResponseTypes =
                {
                    new ApiResponseType
                    {
                        Type = typeof(TestModel),
                        StatusCode = 200
                    }
                }
            };

            var operationDescription = new SwaggerOperationDescription { Operation = new SwaggerOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreToSwaggerGeneratorSettings)context.Settings);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            Assert.Collection(
                context.OperationDescription.Operation.Responses,
                kvp =>
                {
                    Assert.Equal("200", kvp.Key);
                    Assert.NotNull(kvp.Value.Schema);
                });
        }

        [Fact]
        public async Task ProcessAsync_AddsResponseForDefaultStatusCode()
        {
            // Arrange
            var apiDescription = new ApiDescription
            {
                SupportedResponseTypes =
                {
                    new ApiResponseType
                    {
                        Type = typeof(TestModel),
                        StatusCode = 0,
                        IsDefaultResponse = true,

                    }
                }
            };

            var operationDescription = new SwaggerOperationDescription { Operation = new SwaggerOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreToSwaggerGeneratorSettings)context.Settings);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            Assert.Collection(
                context.OperationDescription.Operation.Responses,
                kvp =>
                {
                    Assert.Equal("default", kvp.Key);
                    Assert.NotNull(kvp.Value.Schema);
                });
        }

        [Fact]
        public async Task ProcessAsync_Adds200StatusCodeForVoidResponse()
        {
            // Arrange
            var apiDescription = new ApiDescription
            {
                SupportedResponseTypes =
                {
                    new ApiResponseType
                    {
                        Type = typeof(void),
                        StatusCode = 0,
                    }
                }
            };

            var operationDescription = new SwaggerOperationDescription { Operation = new SwaggerOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreToSwaggerGeneratorSettings)context.Settings);

            // Act
            await processor.ProcessAsync(context);

            // Assert
            Assert.Collection(
                context.OperationDescription.Operation.Responses,
                kvp =>
                {
                    Assert.Equal("200", kvp.Key);
                    Assert.Null(kvp.Value.Schema);
                });
        }

        private AspNetCoreOperationProcessorContext GetContext(ApiDescription apiDescription)
        {
            var operationDescription = new SwaggerOperationDescription { Operation = new SwaggerOperation() };
            var swaggerSettings = new AspNetCoreToSwaggerGeneratorSettings();
            var document = new SwaggerDocument();
            var generator = new AspNetCoreToSwaggerGenerator(swaggerSettings);
            var schemaGeneratorSettings = new JsonSchemaGeneratorSettings();
            var schemaGenerator = new JsonSchemaGenerator(schemaGeneratorSettings);
            var schemaResolver = new SwaggerSchemaResolver(document, schemaGeneratorSettings);
            var context = new AspNetCoreOperationProcessorContext(
                document,
                operationDescription,
                GetType(),
                GetType().GetMethod(nameof(SomeAction), BindingFlags.NonPublic | BindingFlags.Instance),
                new SwaggerGenerator(schemaGenerator, schemaGeneratorSettings, schemaResolver),
                schemaGenerator,
                schemaResolver,
                swaggerSettings,
                new List<SwaggerOperationDescription>())
            {
                ApiDescription = apiDescription,
            };
            return context;
        }

        private class TestModel { }

        private TestModel SomeAction() => null;
    }
}
