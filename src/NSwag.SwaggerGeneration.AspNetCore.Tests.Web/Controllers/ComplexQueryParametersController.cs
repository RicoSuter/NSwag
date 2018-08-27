using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/complexqueryparameters")]
    public class ComplexQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList([FromQuery]GetListCommand getListCommand)
        {
            return Ok();
        }

        public class GetListCommand
        {
            public int? Page { get; set; }

            public int PageSize { get; set; } = 10;
        }
    }
}