using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList(int? required, int optional = 10, [BindRequired, FromQuery] string requiredString = null)
        {
            return Ok();
        }
    }
}