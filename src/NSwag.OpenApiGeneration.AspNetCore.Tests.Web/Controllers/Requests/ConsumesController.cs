using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Requests
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumesController : Controller
    {
        [Consumes("foo/bar", "text/html")] // requires CustomTextInputFormatter
        [HttpPost("ConsumesOnOperation")]
        public ActionResult ConsumesOnOperation([FromBody] string body)
        {
            return Ok();
        }

        [Consumes("text/html")]
        [HttpPost("SecondOperation")]
        public ActionResult SecondOperation([FromBody] string body)
        {
            return Ok();
        }
    }
}
