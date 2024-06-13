﻿using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NJsonSchema;
using NJsonSchema.NewtonsoftJson.Generation;
using NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters;
using Xunit;
using static NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters.DefaultParametersController;

namespace NSwag.Generation.AspNetCore.Tests.Parameters
{
    public class DefaultParametersTests : AspNetCoreTestsBase
    {
        [Fact]
        public async Task When_parameter_has_default_and_schema_type_is_OpenApi3_then_schema_default_is_set()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(DefaultParametersController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(DefaultParametersController.WithDefault))).Operation;

            Assert.Equal(5, operation.Parameters.First().Schema.Default);
        }

        [Fact]
        public async Task When_parameter_has_default_and_schema_type_is_OpenApi3_then_schema_default_is_set_on_oneOf_reference()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(DefaultParametersController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(DefaultParametersController.WithDefaultEnum))).Operation;

            Assert.Equal((int)MyEnum.Def, operation.Parameters.First().Schema.Default);
            Assert.True(operation.Parameters.First().Schema.OneOf.Any());
        }

        [Fact]
        public async Task When_parameter_has_default_and_schema_type_is_OpenApi3_then_schema_default_is_set_and_is_string()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.OpenApi3,
                    SerializerSettings = new JsonSerializerSettings
                    {
                        Converters = { new StringEnumConverter() }
                    }
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(DefaultParametersController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(DefaultParametersController.WithDefaultEnum))).Operation;

            Assert.Equal("Def", operation.Parameters.First().Schema.Default);
        }

        [Fact]
        public async Task When_parameter_has_default_and_schema_type_is_Swagger2_then_parameter_default_is_set()
        {
            // Arrange
            var settings = new AspNetCoreOpenApiDocumentGeneratorSettings
            {
                RequireParametersWithoutDefault = true,
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = SchemaType.Swagger2
                }
            };

            // Act
            var document = await GenerateDocumentAsync(settings, typeof(DefaultParametersController));
            var json = document.ToJson();

            // Assert
            var operation = document.Operations.First(o => o.Path.Contains(nameof(DefaultParametersController.WithDefault))).Operation;

            Assert.Null(operation.Parameters.First().Schema);
            Assert.Equal(5, operation.Parameters.First().Default);
        }
    }
}