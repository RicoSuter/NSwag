using System.Threading.Tasks;
using NJsonSchema;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class CallbacksSerializationTests
    {
        [Fact]
        public async Task When_callbacks_are_defined_then_they_are_serialized_in_OpenApi()
        {
            //// Arrange
            var document = new SwaggerDocument
            {
                Paths =
                {
                    {
                        "/baz",
                        new SwaggerPathItem
                        {
                            {
                                SwaggerOperationMethod.Get,
                                new SwaggerOperation
                                {
                                    Callbacks =
                                    {
                                        {
                                            "onData",
                                            new OpenApiCallback
                                            {
                                                {
                                                    "foo",
                                                    new SwaggerPathItem
                                                    {
                                                        {
                                                            SwaggerOperationMethod.Post,
                                                            new SwaggerOperation
                                                            {
                                                                Description = "bar"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            //// Act
            var json = document.ToJson(SchemaType.OpenApi3);

            //// Assert
            Assert.Contains(@"""bar""", json);
        }
    }
}