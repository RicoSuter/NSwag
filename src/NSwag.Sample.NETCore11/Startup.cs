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

            // Add NSwag OpenAPI/Swagger services
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();

            // Add Swagger middlewares

            app.UseSwagger(options => options.Path = "/swagger/v1/swagger.json");

            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoute = "/swagger/v1/swagger.json";
                options.SwaggerUiRoute = "/swagger_ui3";
            });

            app.UseReDoc(options =>
            {
                options.SwaggerRoute = "/swagger/v1/swagger.json";
                options.SwaggerUiRoute = "/swagger_redoc";
            });
        }
    }
}