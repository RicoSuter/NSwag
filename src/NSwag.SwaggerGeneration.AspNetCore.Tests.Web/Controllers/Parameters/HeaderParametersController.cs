using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeaderParametersController : Controller
    {
        [HttpGet]
        public ActionResult MyAction([FromHeader] string required, [FromHeader] string optional = null)
        {
            return Ok();
        }
    }
}