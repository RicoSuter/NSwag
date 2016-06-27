using System.Web.Http;
using Microsoft.Owin;
using NSwag.AspNet.Owin;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Demo.OwinWeb;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace NSwag.Demo.OwinWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            app.UseSwaggerUi(typeof(Startup).Assembly, new SwaggerUiOwinSettings
            {
                Title = "NSwag Sample API"
            });
            app.UseWebApi(config);

            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
        }
    }
}