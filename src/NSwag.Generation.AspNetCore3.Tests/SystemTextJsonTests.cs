using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using NSwag.AspNetCore;
using Xunit;

namespace NSwag.Generation.AspNetCore3.Tests
{
    public class SystemTextJsonTests
    {
        [Fact]
        public async Task WhenSystemTextOptionsIsUsed_ThenOptionsAreConverted()
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
    }
}
