using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
            [BindRequired]
            public int? Required { get; set; }

            public int Optional { get; set; } = 10;
        }
    }
}