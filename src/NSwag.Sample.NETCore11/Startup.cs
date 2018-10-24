using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSwag.AspNetCore;

namespace NSwag.Sample.NETCore11
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new YamlOutputFormatter());
            });

            services.Configure<SwaggerUi3Options>(options => options.UiRouteTemplate = "/swagger_api_ui3");
            SwaggerServiceCollectionExtensions.AddSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            // API Explorer based (new)

            var redocOptions = new SwaggerMiddlewareOptions
            {
                SwaggerRoute = "/swagger_api_redoc/{documentName}/swagger.json",
            };

            app.UseSwagger(redocOptions);
            app.UseSwaggerReDocUi("/swagger_api_redoc/{documentName}", redocOptions);

            var options = new SwaggerMiddlewareOptions
            {
                SwaggerRoute = "/swagger_api_ui3/{documentName}/swagger.json",
            };

            app.UseSwagger(options);
            app.UseSwaggerUi4(options: null, generationMiddlewareOptions: options);

#pragma warning disable CS0618 // Type or member is obsolete
            app.UseSwaggerUiWithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_api_ui/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_api_ui";
            });

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
#pragma warning restore CS0618 // Type or member is obsolete

            // Tests

            app.UseSwagger();
        }
    }
}
