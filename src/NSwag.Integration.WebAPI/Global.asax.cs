using System.Web.Http;
using System.Web.Routing;
using NSwag.AspNet.Owin;

namespace NSwag.Integration.WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapOwinPath("swagger", app =>
            {
                app.UseReDoc(typeof(WebApiApplication).Assembly, new SwaggerUiSettings
                {
                    MiddlewareBasePath = "/swagger", 
                    DefaultUrlTemplate = "api/{controller}/{action}/{id}"
                });
            });


            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
