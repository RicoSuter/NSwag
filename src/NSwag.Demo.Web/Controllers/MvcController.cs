using System;
using System.Web.Mvc;

namespace NSwag.Demo.Web.Controllers
{
    // Shold not be picked up by GetControllerClasses()
    public class MvcController : Controller
    {
        public ActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}