using System.Linq;
using Namotion.Reflection;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
using Xunit;

namespace NSwag.Generation.Tests
{
    public class OpenApiDocumentGeneratorTests
    {

        public class TestController
        {
            public void HasArrayParameter(string[] foo)
            {
            }
        }

        private OpenApiParameter GetParameter(SchemaType schemaType)
        {
            var generatorSettings = new OpenApiDocumentGeneratorSettings
            {
                SchemaSettings = new NewtonsoftJsonSchemaGeneratorSettings
                {
                    SchemaType = schemaType,
                }
            };

            var schemaResolver = new JsonSchemaResolver(new OpenApiDocument(), generatorSettings.SchemaSettings);
            var generator = new OpenApiDocumentGenerator(generatorSettings, schemaResolver);
            var methodInfo = typeof(TestController)
                .ToContextualType()
                .Methods
                .Single(m => m.Name == "HasArrayParameter");

            return generator.CreatePrimitiveParameter("foo", "bar", methodInfo.Parameters.First());
        }

        [Fact]
        public void CreatePrimitiveParameter_QueryStringArray_OpenApi()
        {
            var parameter = GetParameter(SchemaType.OpenApi3);

            Assert.True(parameter.Explode);
            Assert.Equal(OpenApiParameterStyle.Form, parameter.Style);
            Assert.Equal(OpenApiParameterCollectionFormat.Undefined, parameter.CollectionFormat);
        }

        [Fact]
        public void CreatePrimitiveParameter_QueryStringArray_Swagger()
        {
            var parameter = GetParameter(SchemaType.Swagger2);

            Assert.False(parameter.Explode);
            Assert.Equal(OpenApiParameterStyle.Undefined, parameter.Style);
            Assert.Equal(OpenApiParameterCollectionFormat.Multi, parameter.CollectionFormat);
        }
    }
}