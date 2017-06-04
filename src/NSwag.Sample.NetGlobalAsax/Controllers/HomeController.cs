using NSwag.Annotations;
using System.Web.Mvc;

namespace NSwag.Sample.NetOwin.Controllers
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
