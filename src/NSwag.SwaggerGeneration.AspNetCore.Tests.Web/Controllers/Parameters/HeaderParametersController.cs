using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeaderParametersController : Controller
    {
        [HttpGet]
        public ActionResult MyAction([FromHeader] string first, [FromHeader] string second = null)
        {
            return Ok();
        }
    }
}