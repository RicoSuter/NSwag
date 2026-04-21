using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimpleQueryParametersController : Controller
    {
        [HttpGet]
        public ActionResult GetList(int requiredBecauseNonNullable, int? optionalBecauseNullable, int? optionalBecauseNullableWithDefaultValue = 10, [BindRequired, FromQuery] string? requiredBecauseBindRequired = null)
        {
            return Ok();
        }
    }
}