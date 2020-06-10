using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NJsonSchema.Generation;

namespace NSwag.Sample.NETCore30
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

            app.UseOpenApi();
            app.UseSwaggerUi3();
            //app.UseApiverse();
            app.UseApiverse(settings =>
            {
                //settings.CompareTo = "a:a:27:25:15:latest";
                settings.ApiverseUrl = "https://localhost:5001";
            });
        }
    }
}
