using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NSwag.AspNet.Owin;

namespace NSwag.Sample.NetGlobalAsax
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapOwinPath("swagger", app =>
            {
                app.UseSwaggerReDoc(typeof(WebApiApplication).Assembly, s =>
                {
                    s.GeneratorSettings.DefaultUrlTemplate = "api/{controller}/{id}";
                    s.MiddlewareBasePath = "/swagger";
                });
            });

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
