using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Requests
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostBodyController : Controller
    {
        [OpenApiBodyParameter]
        [HttpPost("JsonPostBodyOperation")]
        public ActionResult JsonPostBodyOperation()
        {
            return Ok();
        }

        [OpenApiBodyParameter("text/plain")]
        [HttpPost("FilePostBodyOperation")]
        public ActionResult FilePostBodyOperation()
        {
            return Ok();
        }
    }
}
