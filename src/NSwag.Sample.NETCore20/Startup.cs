using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using NSwag.Sample.NETCore20.Part;
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

            // Add OpenAPI and Swagger DI services and configure documents

            // Adds the NSwag services
            services.AddSwagger(options => options
                // Register a Swagger 2.0 document generator
                .AddSwaggerDocument(settings =>
                {
                    settings.DocumentName = "swagger";
                    // Add custom document processors, etc.
                    settings.DocumentProcessors.Add(new SecurityDefinitionAppender("TEST_HEADER", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "TEST_HEADER",
                        In = SwaggerSecurityApiKeyLocation.Header,
                        Description = "TEST_HEADER"
                    }));
                })
                // Register an OpenAPI 3.0 document generator
                .AddOpenApiDocument(settings => settings.DocumentName = "openapi"));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            //// Add OpenAPI and Swagger middlewares to serve documents and web UIs

            // Add Swagger 2.0 document serving middleware
            app.UseSwagger(options =>
            {
                options.DocumentName = "swagger";
                options.Path = "/swagger/v1/swagger.json";
            });

            // Add web UIs to interact with the document
            app.UseSwaggerUi3(options =>
            {
                // Define OpenAPI/Swagger document route (defined with UseSwaggerWithApiExplorer)
                options.SwaggerRoute = "/swagger/v1/swagger.json";

                // Define web UI route
                options.SwaggerUiRoute = "/swagger_ui";
            });
            app.UseReDoc(options =>
            {
                options.SwaggerRoute = "/swagger/v1/swagger.json";
                options.SwaggerUiRoute = "/swagger_redoc";
            });

            //// Add OpenAPI 3.0 document serving middleware
            app.UseSwagger(options =>
            {
                options.DocumentName = "swagger";
                options.Path = "/openapi/v1/openapi.json";
            });

            // Add web UIs to interact with the document
            app.UseSwaggerUi3(options =>
            {
                options.SwaggerRoute = "/openapi/v1/openapi.json";
                options.SwaggerUiRoute = "/openapi_ui";
            });
            app.UseReDoc(options =>
            {
                options.SwaggerRoute = "/openapi/v1/openapi.json";
                options.SwaggerUiRoute = "/openapi_redoc";
            });

            // Add Swagger UI with multiple documents
            app.UseSwaggerUi3(options =>
            {
                // Add multiple OpenAPI/Swagger documents to the Swagger UI 3 web frontend
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Swagger", "/swagger/v1/swagger.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Openapi", "/openapi/v1/openapi.json"));
                options.SwaggerRoutes.Add(new SwaggerUi3Route("Petstore", "http://petstore.swagger.io/v2/swagger.json"));

                // Define web UI route
                options.SwaggerUiRoute = "/swagger_all";
            });
        }
    }
}