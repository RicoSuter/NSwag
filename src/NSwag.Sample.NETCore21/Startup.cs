using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.AspNetCore;

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

            // Add OpenAPI services

            services.AddSwagger(options => options.AddDocument(settings => settings.SchemaType = SchemaType.OpenApi3));
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

            // Add OpenAPI middlewares

            app.UseSwaggerUi3();
            app.UseSwaggerWithApiExplorer();
        }
    }
}
