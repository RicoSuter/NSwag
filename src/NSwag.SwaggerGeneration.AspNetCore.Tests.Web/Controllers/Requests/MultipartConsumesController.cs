using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Requests
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultipartConsumesController : Controller
    {
        [Consumes("multipart/form-data")]
        [HttpPost("ConsumesOnOperation")]
        public ActionResult ConsumesOnOperation([FromBody] string body)
        {
            return Ok();
        }
    }
}
