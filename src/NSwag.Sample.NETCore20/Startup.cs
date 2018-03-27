using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;

namespace NSwag.Sample.NETCore20
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

            services.AddSwagger();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // API Explorer based (new)

            app.UseSwaggerUiWithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_api_ui/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_api_ui";
            });

            app.UseSwaggerUi3WithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_api_ui3/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_api_ui3";
            });

            app.UseSwaggerReDocWithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_api_redoc/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_api_redoc";
            });

            // Reflection based (old)

            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_ui/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_ui";
            });

            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_ui3/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_ui3";
            });

            app.UseSwaggerReDoc(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_redoc/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_redoc";
            });
        }
    }
}
