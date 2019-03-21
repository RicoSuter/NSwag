using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web
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
                .AddMvc(config =>
                {
                    config.InputFormatters.Add(new CustomTextInputFormatter());
                    config.OutputFormatters.Add(new CustomTextOutputFormatter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvcCore()
            .AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services
                .AddSwaggerDocument(document =>
                {
                    document.DocumentName = "v1";
                    document.ApiGroupNames = new[] { "1" };
                })
                .AddSwaggerDocument(document =>
                {
                    document.DocumentName = "v2";
                    document.ApiGroupNames = new[] { "2" };
                })
                .AddSwaggerDocument(document =>
                {
                    document.DocumentName = "v3";
                    document.ApiGroupNames = new[] { "3" };
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

            app.UseSwagger();
            app.UseSwaggerUi3();
        }
    }
}
