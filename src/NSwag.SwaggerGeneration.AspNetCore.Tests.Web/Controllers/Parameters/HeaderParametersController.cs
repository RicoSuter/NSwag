using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class HeaderParametersController : Controller
    {
        [HttpGet]
        public ActionResult MyAction([FromHeader, BindRequired] string required, [FromHeader] string optional = null)
        {
            return Ok();
        }
    }
}