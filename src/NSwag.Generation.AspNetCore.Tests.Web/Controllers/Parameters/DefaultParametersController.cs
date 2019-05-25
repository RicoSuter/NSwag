using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class DefaultParametersController : Controller
    {
        [HttpGet("WithDefault")]
        public ActionResult WithDefault(int? parameter = 5)
        {
            return Ok();
        }

        [HttpGet("WithDefaultEnum")]
        public ActionResult WithDefaultEnum(MyEnum parameter = MyEnum.Def)
        {
            return Ok();
        }

        public enum MyEnum
        {
            Abc,
            Def
        }
    }
}