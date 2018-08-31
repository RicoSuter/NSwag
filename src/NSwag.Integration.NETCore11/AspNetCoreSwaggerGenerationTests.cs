//-----------------------------------------------------------------------
// <copyright file="AspNetCoreToSwaggerGenerator.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using NJsonSchema;
using Xunit;

namespace NSwag.Integration.NETCore11
{
    public class AspNetCoreSwaggerGenerationTests : IDisposable
    {
        private readonly TestServer _testServer;

        public AspNetCoreSwaggerGenerationTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Sample.NETCore11.Startup>());
            Client = _testServer.CreateClient();
            Client.BaseAddress = _testServer.BaseAddress;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task SwaggerGeneration_IncludesAllOperations()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();

            // Assert
            Assert.Collection(
                document.Operations,
                operation =>
                {
                    Assert.Equal("/pet", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Post, operation.Method);
                },
                operation =>
                {
                    Assert.Equal("/pet", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Put, operation.Method);
                },
                operation =>
                {
                    Assert.Equal("/pet/findByStatus/{skip}/{sortOrder}", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Get, operation.Method);
                },
                operation =>
                {
                    Assert.Equal("/pet/findByTags", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Get, operation.Method);
                },
                operation =>
                {
                    Assert.Equal("/pet/{id}", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Delete, operation.Method);
                },
                operation =>
                {
                    Assert.Equal("/pet/{id}", operation.Path);
                    Assert.Equal(SwaggerOperationMethod.Get, operation.Method);
                });
        }

        [Fact]
        public async Task SwaggerGeneration_IncludesMetadataForComplexTypes()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();

            // Assert
            var method = Assert.Single(
                document.Operations, 
                operation => operation.Path == "/pet" && operation.Method == SwaggerOperationMethod.Post);
            Assert.Collection(
                method.Operation.Parameters,
                parameter =>
                {
                    Assert.Equal("pet", parameter.Name);
                    Assert.True(parameter.IsRequired);
                    Assert.Equal("The Pet to create.", parameter.Description);
                    Assert.Equal(SwaggerParameterKind.Body, parameter.Kind);
                    Assert.Equal("#/definitions/Pet", parameter.ActualSchema.ExtensionData["$ref"]);
                });
        }

        [Fact]
        public async Task SwaggerGeneration_IncludesMetadataForPrimitiveAndEnumTypes()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();
            var json = document.ToJson();

            // Assert
            var method = Assert.Single(
                document.Operations,
                operation => operation.Path == "/pet/findByStatus/{skip}/{sortOrder}");

            Assert.Collection(
                method.Operation.Parameters,
                parameter =>
                {
                    Assert.Equal("status", parameter.Name);
                    Assert.True(parameter.IsRequired);
                    Assert.Equal(SwaggerParameterKind.Query, parameter.Kind);
                    Assert.Equal("#/definitions/Status", parameter.ActualSchema.ExtensionData["$ref"]);
                    Assert.Equal(new [] { "0", "1", "2" }, parameter.Enumeration.Select(e => e.ToString()));
                },
                parameter =>
                {
                    Assert.Equal("skip", parameter.Name);
                    Assert.True(parameter.IsRequired);
                    Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
                    Assert.Equal(JsonObjectType.Integer, parameter.Type);
                },
                parameter =>
                {
                    Assert.Equal("sortOrder", parameter.Name);
                    Assert.True(parameter.IsRequired); // path parameters are always true
                    Assert.Equal(SwaggerParameterKind.Path, parameter.Kind);
                    Assert.Equal(JsonObjectType.String, parameter.Type);
                });
        }

        [Fact]
        public async Task SwaggerGeneration_IncludesArrayParameters()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();

            // Assert
            var method = Assert.Single(
                document.Operations,
                operation => operation.Path == "/pet/findByTags");
            Assert.Collection(
                method.Operation.Parameters,
                parameter =>
                {
                    Assert.Equal("tags", parameter.Name);
                    Assert.True(parameter.IsRequired);
                    Assert.Equal(SwaggerParameterKind.Query, parameter.Kind);
                    Assert.Equal(SwaggerParameterCollectionFormat.Multi, parameter.CollectionFormat);
                    Assert.True(parameter.ActualSchema.IsArray);
                    Assert.Equal(JsonObjectType.String, parameter.ActualSchema.Item.Type);
                });
        }

        [Fact]
        public async Task SwaggerGeneration_IncludesResponseTypes()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();

            // Assert
            var method = Assert.Single(
                document.Operations,
                operation => operation.Path == "/pet/{id}" && operation.Method == SwaggerOperationMethod.Get);
            Assert.Collection(
                method.Operation.ActualResponses.OrderBy(kvp => kvp.Key),
                kvp =>
                {
                    Assert.Equal("200", kvp.Key);
                    Assert.Equal("#/definitions/Pet", kvp.Value.ActualResponseSchema.ExtensionData["$ref"]);
                },
                kvp =>
                {
                    Assert.Equal("400", kvp.Key);
                    Assert.Equal("#/definitions/SerializableError", kvp.Value.ActualResponseSchema.ExtensionData["$ref"]);
                },
                kvp =>
                {
                    Assert.Equal("404", kvp.Key);
                });
        }

        [Fact]
        public async Task SwaggerGeneration_IncludesProducesAndConsumes()
        {
            // Arrange & Act
            var document = await GetSwaggerDocument();

            // Assert
            Assert.Equal(new[] { "application/json", "application/yaml", "text/json", "text/plain" }, document.Produces.OrderBy(p => p));
            Assert.Equal(new[] { "application/json", "application/json-patch+json", "text/json" }, document.Consumes.OrderBy(p => p));
        }

        private async Task<SwaggerDocument> GetSwaggerDocument()
        {
            var responseBody = await Client.GetStringAsync("swagger/v1/swagger.json");
            return await SwaggerDocument.FromJsonAsync(responseBody);
        }

        public void Dispose()
        {
            Client?.Dispose();
            _testServer.Dispose();
        }
    }
}
