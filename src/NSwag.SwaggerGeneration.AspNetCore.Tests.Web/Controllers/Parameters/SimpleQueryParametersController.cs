using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList(int? required, int optional = 10)
        {
            return Ok();
        }
    }
}