using NSwag.AspNet.Owin;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace NSwag.Sample.AspNet
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapOwinPath("swagger", app =>
            {
                app.UseSwaggerUi(typeof(WebApiApplication).Assembly, settings =>
                {
                    settings.MiddlewareBasePath = "/swagger";
                    //settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";  //this is the default one, standard ASP.NET Framework routing, but not compatible with Swagger/OpenAPI specification
                    settings.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{action}/{id}";
                });
            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}
