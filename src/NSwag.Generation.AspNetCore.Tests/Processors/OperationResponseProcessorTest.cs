﻿//-----------------------------------------------------------------------
// <copyright file="OperationResponseProcessorTest.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/RicoSuter/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace NSwag.Generation.AspNetCore.Processors.Tests
{
    public class OperationResponseProcessorTest
    {
        [Fact]
        public void ProcessAsync_AddsResponseFor200StatusCode()
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

            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreOpenApiDocumentGeneratorSettings)context.Settings);

            // Act
            processor.Process(context);

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
        public void ProcessAsync_AddsResponseForDefaultStatusCode()
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

            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreOpenApiDocumentGeneratorSettings)context.Settings);

            // Act
            processor.Process(context);

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
        public void ProcessAsync_Adds200StatusCodeForVoidResponse()
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

            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            var context = GetContext(apiDescription);
            var processor = new OperationResponseProcessor((AspNetCoreOpenApiDocumentGeneratorSettings)context.Settings);

            // Act
            processor.Process(context);

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
            var operationDescription = new OpenApiOperationDescription { Operation = new OpenApiOperation() };
            var document = new OpenApiDocument();
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            var schemaResolver = new OpenApiSchemaResolver(document, settings.SchemaSettings);
            var generator = new OpenApiDocumentGenerator(settings, schemaResolver);

            var context = new AspNetCoreOperationProcessorContext(
                document,
                operationDescription,
                GetType(),
                GetType().GetMethod(nameof(SomeAction), BindingFlags.NonPublic | BindingFlags.Instance),
                generator,
                schemaResolver,
                settings,
                new List<OpenApiOperationDescription>())
            {
                ApiDescription = apiDescription,
            };
            return context;
        }

        private class TestModel { }

        private TestModel SomeAction() => null;
    }
}
