using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag.AspNetCore;
using NSwag.Sample.NETCore20.Part;
using NSwag.SwaggerGeneration.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

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
            services.AddMvc().
                AddApplicationPart(typeof(SampleController).GetTypeInfo().Assembly);

            // Add OpenAPI and Swagger services

            services.AddSwagger(options => options
                .AddDocument("swagger", settings =>
                {
                    settings.DocumentProcessors.Add(new SecurityDefinitionAppender("TEST_HEADER", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "TEST_HEADER",
                        In = SwaggerSecurityApiKeyLocation.Header,
                        Description = "TEST_HEADER"
                    }));
                })
                .AddDocument("openapi", settings => settings.SchemaType = SchemaType.OpenApi3));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Add OpenAPI and Swagger middlewares

            // Swagger 2.0
            app.UseSwaggerWithApiExplorer("/swagger/v1/swagger.json", "swagger");

            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoute = "/swagger/v1/swagger.json";
                options.SwaggerUiRoute = "/swagger_ui";
            });
            app.UseSwaggerReDoc(options =>
            {
                options.SwaggerRoute = "/swagger/v1/swagger.json";
                options.SwaggerUiRoute = "/swagger_redoc";
            });

            // OpenAPI 3.0
            app.UseSwaggerWithApiExplorer("/openapi/v1/swagger.json", "openapi");

            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoute = "/openapi/v1/swagger.json";
                options.SwaggerUiRoute = "/openapi_ui";
            });
            app.UseSwaggerReDoc(options =>
            {
                options.SwaggerRoute = "/openapi/v1/swagger.json";
                options.SwaggerUiRoute = "/openapi_redoc";
            });

            // All
            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Swagger", "/swagger/v1/swagger.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Openapi", "/openapi/v1/swagger.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Petstore", "http://petstore.swagger.io/v2/swagger.json"));

                options.SwaggerUiRoute = "/swagger_all";
            });
        }
    }
}
