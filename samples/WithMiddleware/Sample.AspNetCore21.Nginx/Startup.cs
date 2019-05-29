using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SimpleApp
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddSwaggerDocument();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseMvc();

            // There are two ways to run this app: 
            // 1. Run docker-compose and access http://localhost:8080/externalpath/swagger
            // 2. Run Sample.AspNetCore21.Nginx and access http://localhost:59185/swagger/
            // both URLs should be correctly served...

            // Config with support for multiple documents
            app.UseOpenApi(config => config.PostProcess = (document, request) =>
            {
                if (request.Headers.ContainsKey("X-External-Host"))
                {
                    // Change document server settings to public
                    document.Host = request.Headers["X-External-Host"].First();
                    document.BasePath = request.Headers["X-External-Path"].First();
                }
            });

            app.UseSwaggerUi3(config => config.TransformToExternalPath = (internalUiRoute, request) =>
            {
                // The header X-External-Path is set in the nginx.conf file
                var externalPath = request.Headers.ContainsKey("X-External-Path") ? request.Headers["X-External-Path"].First() : "";
                return externalPath + internalUiRoute;
            });

            // Config with single document
            //app.UseSwagger(config =>
            //{
            //    config.Path = "/swagger/v1/swagger.json";
            //    config.PostProcess = (document, request) =>
            //    {
            //        if (request.Headers.ContainsKey("X-External-Host"))
            //        {
            //            // Change document server settings to public
            //            document.Host = request.Headers["X-External-Host"].First();
            //            document.BasePath = request.Headers["X-External-Path"].First();
            //        }
            //    };
            //});
            //app.UseSwaggerUi3(config =>
            //{
            //    config.SwaggerRoute = "/swagger/v1/swagger.json";
            //    config.TransformToExternalPath = (internalUiRoute, request) =>
            //    {
            //        // The header X-External-Path is set in the nginx.conf file
            //        var externalPath = request.Headers.ContainsKey("X-External-Path") ? request.Headers["X-External-Path"].First() : "";
            //        return externalPath + internalUiRoute;
            //    };
            //});
        }
    }
}
