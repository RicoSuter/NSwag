using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NSwag.Sample.NETCore21
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
            services
                .AddMvc(options => options.AllowEmptyInputInBodyModelBinding = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add NSwag OpenAPI/Swagger DI services and configure documents
            // For more advanced setup, see NSwag.Sample.NETCore20 project

            services.AddSwagger(options =>
            {
                options.AddOpenApiDocument(document => document.DocumentName = "a");
                options.AddSwaggerDocument(document => document.DocumentName = "b");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            // Add middlewares to service the OpenAPI/Swagger document and the web UI

            app.UseSwagger(); // registers the two documents in separate routes
            app.UseSwaggerUi3(); // registers a single Swagger UI (v3) with the two documents
        }
    }
}