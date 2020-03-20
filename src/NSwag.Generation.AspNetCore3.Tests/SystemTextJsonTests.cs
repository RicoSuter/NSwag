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
            var services = new ServiceCollection()
                .AddLogging();
            
            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddOpenApiDocument();
            var sp = services.BuildServiceProvider();
            
            var generator = sp.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(sp);
            var settings =   generator.Generator.Settings;

            Assert.Contains(settings.SerializerSettings.Converters,c =>  c is StringEnumConverter);
        }
        
        [Fact]
        public async Task SystemTextJsonCaseOptionIsRead()
        {
            var services = new ServiceCollection()
                .AddLogging();
            
            services.AddControllers()
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null);
            services.AddOpenApiDocument();
            var sp = services.BuildServiceProvider();
            
            var generator = sp.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(sp);
            var settings =   generator.Generator.Settings;

            Assert.IsType<DefaultContractResolver>(settings.SerializerSettings.ContractResolver);
        }
        
        [Fact]
        public async Task SystemTextJsonOptionDefaultsWhenNotSet()
        {
            var services = new ServiceCollection()
                .AddLogging();

            services.AddControllers();
            services.AddOpenApiDocument();
            var sp = services.BuildServiceProvider();
            
            var generator = sp.GetRequiredService<OpenApiDocumentRegistration>();
            await generator.Generator.GenerateAsync(sp);
            var settings =   generator.Generator.Settings;

            Assert.IsType<CamelCasePropertyNamesContractResolver>(settings.SerializerSettings.ContractResolver);
            Assert.DoesNotContain(settings.SerializerSettings.Converters,c =>  c is StringEnumConverter);
        }
    }
}
