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

            // Swagger v2.0

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

            app.UseSwaggerUi3WithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_new_ui3/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_new_ui3";

                s.GeneratorSettings.DocumentProcessors.Add(new SecurityDefinitionAppender("TEST_HEADER", new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Name = "TEST_HEADER",
                    In = SwaggerSecurityApiKeyLocation.Header,
                    Description = "TEST_HEADER"
                }));
            });

            app.UseSwaggerReDocWithApiExplorer(s =>
            {
                s.SwaggerRoute = "/swagger_new_redoc/v1/swagger.json";
                s.SwaggerUiRoute = "/swagger_new_redoc";
            });

            // Swagger 3.0

            app.UseSwaggerWithApiExplorer(s =>
            {
                s.GeneratorSettings.SchemaType = SchemaType.OpenApi3;
                s.SwaggerRoute = "/swagger_new_v3/v1/swagger.json";
            });

            // Reflection based (old)

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

            // All
            app.UseSwaggerUi3(s =>
            {
                s.SwaggerRoutes.Add(new SwaggerUi3Route("A", "/swagger_new_ui/v1/swagger.json"));
                s.SwaggerRoutes.Add(new SwaggerUi3Route("B", "http://petstore.swagger.io/v2/swagger.json"));
                s.SwaggerUiRoute = "/swagger_all";
            });
        }
    }
}
