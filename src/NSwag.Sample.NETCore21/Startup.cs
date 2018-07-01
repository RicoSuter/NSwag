using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration;
using NSwag.SwaggerGeneration.AspNetCore;

namespace NSwag.Sample.NETCore21
{
    public class MySettingsFactory : SwaggerGeneratorSettingsFactoryBase<AspNetCoreToSwaggerGeneratorSettings, IServiceProvider>
    {
        protected override async Task ConfigureAsync(AspNetCoreToSwaggerGeneratorSettings settings, IServiceProvider context)
        {
            settings.Title = "Hello from settings factory!";
        }
    }

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

            services.AddSwagger();
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

            app.UseSwaggerWithApiExplorer(settings => settings.SettingsFactory = new MySettingsFactory());
            app.UseSwagger(typeof(Startup).Assembly, settings => settings.SwaggerRoute = "/oldswagger.json");
        }
    }
}
