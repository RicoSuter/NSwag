using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class NonNullablePathParameterControl : Controller
    {
        [HttpGet("{nonNullable}/{nullable}")]
        public ActionResult Get(string nonNullable, string? nullable)
        {
            return Ok();
        }
    }
}
