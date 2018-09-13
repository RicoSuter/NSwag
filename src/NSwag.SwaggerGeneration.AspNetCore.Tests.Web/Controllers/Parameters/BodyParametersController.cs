using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodyParametersController : Controller
    {
        [HttpPost("required")]
        public ActionResult Required([FromBody] string required)
        {
            return Ok();
        }

        [HttpPost("optional")]
        public ActionResult Optional([FromBody] string optional = null)
        {
            return Ok();
        }
    }
}