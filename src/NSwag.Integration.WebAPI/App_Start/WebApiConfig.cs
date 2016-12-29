using System.Linq;
using System.Web.Http;
using NSwag.AspNet.WebApi;

namespace NSwag.Integration.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var xmlFormatter = config.Formatters
                .Where(f => f.SupportedMediaTypes.Any(m => m.MediaType == "application/xml"))
                .ToList().First();

            config.Formatters.Remove(xmlFormatter);
            config.Formatters.Insert(0, xmlFormatter);
            config.Formatters.XmlFormatter.UseXmlSerializer = true;

            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            GlobalConfiguration.Configuration.Filters.Add(new JsonExceptionFilterAttribute(false));
        }
    }
}
