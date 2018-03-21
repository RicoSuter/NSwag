using System.Web.Mvc;
using NSwag.Annotations;

namespace NSwag.Sample.NetGlobalAsax.Controllers
{
    [SwaggerIgnore]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
