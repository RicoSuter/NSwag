using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NJsonSchema.Generation;
using System.Text.Json.Serialization;

namespace NSwag.Sample.NET70
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddOpenApiDocument(document =>
            {
                document.Description = "Hello world!";
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi(p => p.Path = "/swagger/{documentName}/swagger.yaml");
            app.UseSwaggerUi3(p => p.DocumentPath = "/swagger/{documentName}/swagger.yaml");
            //app.UseApimundo();
            app.UseApimundo(settings =>
            {
                //settings.CompareTo = "a:a:27:25:15:latest";
                settings.DocumentPath = "/swagger/v1/swagger.yaml";
                settings.ApimundoUrl = "https://localhost:5001";
            });
        }
    }
}
