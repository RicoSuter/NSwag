using System.Linq;
using System.Threading.Tasks;
using NJsonSchema;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class ServersSerializationTests
    {
        [Fact]
        public async Task When_document_is_empty_then_serialized_correctly_in_Swagger()
        {
            //// Arrange
            var document = new SwaggerDocument();

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Equal(
@"{
  ""swagger"": ""2.0"",
  ""info"": {
    ""title"": """",
    ""version"": """"
  }
}".Replace("\r", ""), json.Replace("\r", ""));
        }

        [Fact]
        public async Task When_schema_is_added_to_definitions_then_it_is_serialized_correctly_in_Swagger()
        {
            //// Arrange
            var document = CreateDocument();

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Equal("rsuter.com", document.Host);
            Assert.Equal("/myapi", document.BasePath);
            Assert.Equal(2, document.Schemes.Count);
            Assert.Equal(SwaggerSchema.Http, document.Schemes.First());
            Assert.Equal(SwaggerSchema.Https, document.Schemes.Last());

            Assert.Contains(@"""basePath""", json);
        }

        [Fact]
        public async Task When_schema_is_added_to_definitions_then_it_is_serialized_correctly_in_OpenApi()
        {
            //// Arrange
            var document = CreateDocument();

            //// Act
            var json = document.ToJson(SchemaType.OpenApi3);

            //// Assert
            Assert.Equal(2, document.Servers.Count);
            Assert.Contains(@"""servers""", json);
        }

        [Fact]
        public async Task When_server_is_set_then_it_is_correctly_converted_to_Swagger()
        {
            //// Arrange
            var document = new SwaggerDocument
            {
                Servers =
                {
                    new OpenApiServer
                    {
                        Url = "http://localhost:12354/myapi"
                    }
                }
            };

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Equal("localhost:12354", document.Host);
            Assert.Equal("/myapi", document.BasePath);
            Assert.Equal(1, document.Schemes.Count);
            Assert.Equal(SwaggerSchema.Http, document.Schemes.First());
        }

        [Fact]
        public async Task When_host_basePath_and_schemeas_are_set_then_it_is_correctly_converted_to_OpenApi()
        {
            //// Arrange
            var document = new SwaggerDocument
            {
                Host = "localhost:12354",
                BasePath = "/myapi",
                Schemes = { SwaggerSchema.Http }
            };

            //// Act
            var json = document.ToJson(SchemaType.Swagger2);

            //// Assert
            Assert.Equal("localhost:12354", document.Host);
            Assert.Equal("/myapi", document.BasePath);
            Assert.Equal(1, document.Schemes.Count);
            Assert.Equal(SwaggerSchema.Http, document.Schemes.First());
        }

        private static SwaggerDocument CreateDocument()
        {
            var document = new SwaggerDocument
            {
                Servers =
                {
                    new OpenApiServer
                    {
                        Url = "http://rsuter.com/myapi"
                    }
                }
            };

            document.Schemes.Add(SwaggerSchema.Https);

            return document;
        }
    }
}