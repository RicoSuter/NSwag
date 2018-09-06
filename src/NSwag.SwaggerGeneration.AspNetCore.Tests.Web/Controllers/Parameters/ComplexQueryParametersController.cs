using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplexQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList([FromQuery]GetListCommand getListCommand)
        {
            return Ok();
        }

        public class GetListCommand
        {
            public int? Required01 { get; set; }

            public int Required02 { get; set; } = 10; // TODO: Should be optional
        }
    }
}