using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.OpenApiGeneration.AspNetCore.Tests.Web.Controllers.Requests
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultipartConsumesController : Controller
    {
        [HttpPost("ConsumesOnOperation")]
        public ActionResult ConsumesOnOperation(IFormFile body)
        {
            return Ok();
        }
    }
}
