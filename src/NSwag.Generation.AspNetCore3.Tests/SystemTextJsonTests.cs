using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using Xunit;

namespace NSwag.Generation.AspNetCore3.Tests
{
    public class SystemTextJsonTests
    {
        [Fact]
        public async Task SystemTextJsonEnumOptionIsRead()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddLogging();

            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddOpenApiDocument();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var generator = serviceProvider.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(serviceProvider);
            var settings = generator.Generator.Settings;

            // Assert
            Assert.Contains(settings.SerializerSettings.Converters, c => c is StringEnumConverter);
        }

        [Fact]
        public async Task SystemTextJsonCaseOptionIsRead()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddLogging();

            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddOpenApiDocument();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var generator = serviceProvider.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(serviceProvider);
            var settings = generator.Generator.Settings;

            // Assert
            Assert.IsType<DefaultContractResolver>(settings.SerializerSettings.ContractResolver);
        }

        [Fact]
        public async Task SystemTextJsonOptionDefaultsWhenNotSet()
        {
            // Arrange
            var services = new ServiceCollection()
                .AddLogging();

            services.AddControllers();
            services.AddOpenApiDocument();
            var serviceProvider = services.BuildServiceProvider();

            // Act
            var generator = serviceProvider.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(serviceProvider);
            var settings = generator.Generator.Settings;

            // Assert
            Assert.IsType<CamelCasePropertyNamesContractResolver>(settings.SerializerSettings.ContractResolver);
            Assert.DoesNotContain(settings.SerializerSettings.Converters, c => c is StringEnumConverter);
        }
    }
}
