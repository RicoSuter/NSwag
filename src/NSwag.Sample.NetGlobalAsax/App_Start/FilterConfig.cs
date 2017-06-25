using System.Web;
using System.Web.Mvc;

namespace NSwag.Sample.NetOwin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
