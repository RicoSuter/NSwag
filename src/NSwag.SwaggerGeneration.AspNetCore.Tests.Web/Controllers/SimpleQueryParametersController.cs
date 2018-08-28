using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/complexqueryparameters")]
    public class SimpleQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList(int? page, int pageSize = 10)
        {
            return Ok();
        }
    }
}