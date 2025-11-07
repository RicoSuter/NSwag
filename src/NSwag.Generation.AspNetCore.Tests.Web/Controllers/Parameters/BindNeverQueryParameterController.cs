using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class BindNeverQueryParameterController : Controller
    {
        [HttpGet]
        public ActionResult Get(string a = null, [BindNever] string b = null, [BindingBehavior(BindingBehavior.Never)] string c = null)
        {
            return Ok();
        }
    }
}