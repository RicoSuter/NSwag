using NJsonSchema;
using Xunit;

namespace NSwag.Core.Tests.Serialization
{
    public class RequestBodySerializationTests
    {
        [Fact]
        public async Task When_request_body_is_added_then_serialized_correctly_in_Swagger()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.Swagger2);
            document = await OpenApiDocument.FromJsonAsync(json);

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;
            Assert.Equal("foo", requestBody.Name);
        }

        [Fact]
        public async Task When_request_body_is_added_then_serialized_correctly_in_OpenApi()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;
            Assert.Equal("foo", requestBody.Name);
        }

        [Fact]
        public async Task When_body_parameter_is_changed_then_request_body_IsRequired_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            parameter.IsRequired = true;

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            Assert.True(requestBody.IsRequired);
            Assert.True(parameter.IsRequired);
        }

        [Fact]
        public async Task When_body_parameter_is_changed_then_request_body_Name_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            parameter.Name += "123";

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            Assert.Equal("foo123", requestBody.Name);
        }

        [Fact]
        public async Task When_body_parameter_is_changed_then_request_body_Schema_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            parameter.Schema = new JsonSchema { Title = "blub" };

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            Assert.Equal("blub", requestBody.Content["application/json"].Schema.Title);
        }

        [Fact]
        public async Task When_body_parameter_is_changed_then_request_body_Description_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            parameter.Description += "123";

            // Assert
            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            Assert.Equal("bar123", requestBody.Description);
        }

        [Fact]
        public async Task When_request_body_is_changed_then_body_parameter_Name_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            requestBody.Name += "123";

            // Assert
            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.Equal("foo123", parameter.Name);
        }

        [Fact]
        public async Task When_request_body_is_changed_then_body_parameter_IsRequired_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;

            requestBody.IsRequired = true;

            // Assert
            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.True(parameter.IsRequired);
            Assert.True(requestBody.IsRequired);
        }

        [Fact]
        public async Task When_request_body_is_changed_then_body_parameter_Content_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;
            requestBody.Content["application/json"] = new OpenApiMediaType
            {
                Schema = new JsonSchema { Title = "blub" }
            };

            // Assert
            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.Equal("blub", parameter.Schema.Title);
        }

        [Fact]
        public async Task When_request_body_is_changed_then_body_parameter_Description_is_updated()
        {
            // Arrange
            var document = CreateDocument();

            // Act
            var json = document.ToJson(SchemaType.OpenApi3);
            document = await OpenApiDocument.FromJsonAsync(json);

            var requestBody = document.Paths["/baz"][OpenApiOperationMethod.Get].RequestBody;
            requestBody.Description += "123";

            // Assert
            var parameter = document.Paths["/baz"][OpenApiOperationMethod.Get].Parameters
                .Single(p => p.Kind == OpenApiParameterKind.Body);

            Assert.Equal("bar123", parameter.Description);
        }

        [Theory]
        [InlineData(SchemaType.OpenApi3)]
        [InlineData(SchemaType.Swagger2)]
        public async Task When_request_body_is_cleared_then_body_parameter_and_serialized_body_are_removed(SchemaType schemaType)
        {
            var document = CreateDocument();
            var operation = document.Paths["/baz"][OpenApiOperationMethod.Get];
            operation.Parameters.Add(new OpenApiParameter { Name = "query", Kind = OpenApiParameterKind.Query, Type = JsonObjectType.String });

            operation.RequestBody = null;

            Assert.Null(operation.RequestBody);
            Assert.DoesNotContain(operation.Parameters, parameter => parameter.Kind == OpenApiParameterKind.Body);
            Assert.Equal("query", Assert.Single(operation.Parameters).Name);

            var reloaded = await OpenApiDocument.FromJsonAsync(document.ToJson(schemaType));
            var reloadedOperation = reloaded.Paths["/baz"][OpenApiOperationMethod.Get];
            Assert.Null(reloadedOperation.RequestBody);
            Assert.DoesNotContain(reloadedOperation.Parameters, parameter => parameter.Kind == OpenApiParameterKind.Body);
            Assert.Equal("query", Assert.Single(reloadedOperation.Parameters).Name);
        }

        [Fact]
        public void When_missing_request_body_is_cleared_then_other_parameters_are_unchanged()
        {
            var operation = new OpenApiOperation();
            var parameter = new OpenApiParameter { Name = "query", Kind = OpenApiParameterKind.Query, Type = JsonObjectType.String };
            operation.Parameters.Add(parameter);

            operation.RequestBody = null;
            operation.RequestBody = null;

            Assert.Null(operation.RequestBody);
            Assert.Same(parameter, Assert.Single(operation.Parameters));
        }

        private static OpenApiDocument CreateDocument()
        {
            var schema = new JsonSchema
            {
                Type = JsonObjectType.String
            };

            var document = new OpenApiDocument
            {
                Paths =
                {
                    {
                        "/baz",
                        new OpenApiPathItem
                        {
                            {
                                OpenApiOperationMethod.Get,
                                new OpenApiOperation
                                {
                                    RequestBody = new OpenApiRequestBody
                                    {
                                        Name = "foo",
                                        Description = "bar",
                                        Content =
                                        {
                                            {
                                                "application/json",
                                                new OpenApiMediaType
                                                {
                                                    Schema = new JsonSchema { Reference = schema }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                Definitions =
                {
                    {
                        "Abc",
                        schema
                    }
                }
            };

            return document;
        }
    }
}
