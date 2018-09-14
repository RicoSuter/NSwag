using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList([BindRequired]int? required, int optional = 10)
        {
            return Ok();
        }
    }
}