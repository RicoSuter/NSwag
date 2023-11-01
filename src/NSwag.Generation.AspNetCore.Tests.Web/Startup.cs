using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace NSwag.Generation.AspNetCore.Tests.Web
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
                .AddControllers(config =>
                {
                    config.InputFormatters.Add(new CustomTextInputFormatter());
                    config.OutputFormatters.Add(new CustomTextOutputFormatter());
                });

            services.AddApiVersioning(options =>
                {
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
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
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi();
        }
    }
}
