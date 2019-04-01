using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Requests
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProducesController : Controller
    {
        [Produces("text/html")]
        [HttpPost("ProducesOnOperation")]
        public ActionResult<string> ProducesOnOperation([FromBody]string body)
        {
            return Ok();
        }
    }
}
