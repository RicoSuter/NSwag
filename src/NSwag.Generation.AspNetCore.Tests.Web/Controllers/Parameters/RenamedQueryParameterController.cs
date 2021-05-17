using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenamedQueryParameterController : Controller
    {
        [HttpGet]
        public ActionResult GetMonths([FromQuery(Name = "month")] string[] months)
        {
            return Ok();
        }
    }
}