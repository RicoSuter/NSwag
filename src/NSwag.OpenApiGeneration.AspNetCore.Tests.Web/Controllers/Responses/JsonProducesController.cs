using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Responses
{
    [ApiController]
    [Route("api/[controller]")]
    public class JsonProducesController : Controller
    {
        [Produces("application/json")]
        [HttpPost("ProducesOnOperation")]
        public ActionResult<string> ProducesOnOperation([FromBody] string body)
        {
            return Ok();
        }
    }
}
