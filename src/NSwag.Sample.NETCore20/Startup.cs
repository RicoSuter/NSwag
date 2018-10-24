using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
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

            services.Configure<SwaggerMiddlewareOptions>(
                options => options.SwaggerRoute = "/swagger_new_ui3/{documentName}/swagger.json");
            services.Configure<SwaggerUi3Options>(options => options.UiRouteTemplate = "/swagger_new_ui3");

            SwaggerServiceCollectionExtensions.AddSwagger(
                services,
                settings => settings.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender(
                    "TEST_HEADER",
                    new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "TEST_HEADER",
                        In = SwaggerSecurityApiKeyLocation.Header,
                        Description = "TEST_HEADER"
                    })))
                .AddOpenApiDocument(settings =>
                {
                    settings.DocumentName = "v3";
                    settings.GeneratorSettings.SchemaType = SchemaType.OpenApi3;
                })
                .AddSwaggerDocument(settings => settings.DocumentName = "A");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // API Explorer based (new)

            // Swagger v2.0

            app.UseSwagger();
            app.UseSwaggerUi4();

            var options2 = new SwaggerMiddlewareOptions
            {
                SwaggerRoute = "/swagger_new_redoc/{documentName}/swagger.json",
            };

            app.UseSwagger(options2);
            app.UseSwaggerReDocUi("v1", "/swagger_new_redoc/{documentName}", options2);

#pragma warning disable CS0618 // Type or member is obsolete
            app.UseSwaggerUiWithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_new_ui/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_new_ui";

                s.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("TEST_HEADER", new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Name = "TEST_HEADER",
                    In = SwaggerSecurityApiKeyLocation.Header,
                    Description = "TEST_HEADER"
                }));
            });
#pragma warning restore CS0618 // Type or member is obsolete

            // Swagger 3.0

            var options3 = new SwaggerMiddlewareOptions
            {
                SwaggerRoute = "/swagger_new_v3/{documentName}/swagger.json",
            };
            var uiOptions3 = new SwaggerUi3Options
            {
                UiRouteTemplate = "/swagger",
            };

            app.UseSwagger(options3);
            app.UseSwaggerUi4(uiOptions3, options3);

            // Reflection based (old)

#pragma warning disable CS0618 // Type or member is obsolete
            app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_old_ui/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_old_ui";
            });

            app.UseSwaggerUi3(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_old_ui3/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_old_ui3";
            });

            app.UseSwaggerReDoc(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.SwaggerRoute = "/swagger_old_redoc/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_old_redoc";
            });

            // Swagger 3.0

            app.UseSwagger(typeof(Startup).GetTypeInfo().Assembly, s =>
            {
                s.GeneratorSettings.SchemaType = SchemaType.OpenApi3;
                s.SwaggerRoute = "/swagger_old_v3/v1/swagger.json";
            });
#pragma warning restore CS0618 // Type or member is obsolete

            // All

            var optionsAll = new SwaggerMiddlewareOptions
            {
                SwaggerRoute = "/swagger_new_ui/{documentName}/swagger.json",
            };
            var uiOptionsAll = new SwaggerUi3Options
            {
                AdditionalSwaggerRoutes =
                {
                    new SwaggerUi3Route("B", "http://petstore.swagger.io/v2/swagger.json")
                },
                UiRouteTemplate = "/swagger_all",
            };

            app.UseSwagger(optionsAll);
            app.UseSwaggerUi4(uiOptionsAll, optionsAll);
        }
    }
}
