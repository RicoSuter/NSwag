using System.Web.Mvc;

namespace NSwag.Sample.AspNet.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return RedirectPermanent("/swagger");
        }
    }
}
