#if NETCOREAPP3_1_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using NSwag.AspNetCore;
using Xunit;
using NJsonSchema.Generation;

namespace NSwag.Generation.AspNetCore.Tests
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
            var registration = serviceProvider.GetRequiredService<OpenApiDocumentRegistration>();
            var generator = new AspNetCoreOpenApiDocumentGenerator(registration.Settings);
            await generator.GenerateAsync(serviceProvider);
         
            var settings = generator.Settings;

            // Assert
            Assert.Contains(((SystemTextJsonSchemaGeneratorSettings)settings.SchemaSettings).SerializerOptions.Converters, c => c is JsonStringEnumConverter);
        }
    }
}
#endif
