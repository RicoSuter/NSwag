using System.Web.Http;
using Microsoft.Owin;
using NSwag.AspNet.Owin;
using Owin;

[assembly: OwinStartup(typeof(NSwag.Sample.NetOwinMiddleware.Startup))]

namespace NSwag.Sample.NetOwinMiddleware
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();

            app.UseSwaggerUi3(typeof(Startup).Assembly, settings =>
            {
                // configure settings here
                // settings.GeneratorSettings.*: Generator settings and extension points
                // settings.*: Routing and UI settings
                settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id?}";
            });
            app.UseWebApi(config);

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.EnsureInitialized();
        }
    }
}
