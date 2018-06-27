using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSwag.SwaggerGeneration.AspNetCore.Tests.Web;
using NSwag.SwaggerGeneration.Processors;
using Xunit;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests
{
    public class ApiVersionProcessorWithAspNetCoreTests : IDisposable
    {
        private readonly TestServer _testServer;

        public ApiVersionProcessorWithAspNetCoreTests()
        {
            _testServer = new TestServer(new WebHostBuilder().UseStartup<Startup>());
        }

        [Fact]
        public async Task When_generating_v1_then_only_v1_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "1" };

            var generator = new AspNetCoreToSwaggerGenerator(settings);
            var provider = _testServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            // Act
            var document = await generator.GenerateAsync(provider.ApiDescriptionGroups);
            var json = document.ToJson();

            // Assert
            Assert.Equal(4, document.Operations.Count());
            Assert.True(document.Operations.All(o => o.Path.Contains("/v1/")));
        }

        [Fact]
        public async Task When_generating_v2_then_only_v2_operations_are_included()
        {
            // Arrange
            var settings = new AspNetCoreToSwaggerGeneratorSettings();
            settings.OperationProcessors.TryGet<ApiVersionProcessor>().IncludedVersions = new[] { "2" };

            var generator = new AspNetCoreToSwaggerGenerator(settings);
            var provider = _testServer.Host.Services.GetRequiredService<IApiDescriptionGroupCollectionProvider>();

            // Act
            var document = await generator.GenerateAsync(provider.ApiDescriptionGroups);
            var json = document.ToJson();

            // Assert
            Assert.Equal(2, document.Operations.Count());
            Assert.True(document.Operations.All(o => o.Path.Contains("/v2/")));
        }

        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}
