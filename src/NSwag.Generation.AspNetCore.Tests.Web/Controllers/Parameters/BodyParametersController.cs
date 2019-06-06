using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [ApiController]
    [Route("api/[controller]")]
    public class BodyParametersController : Controller
    {
        [HttpPost("RequiredPrimitive")]
        public ActionResult RequiredPrimitive([FromBody] string required)
        {
            return Ok();
        }

        [HttpPost("OptionalPrimitive")]
        public ActionResult RequiredPrimitiveWithDefault([FromBody] string required = null)
        {
            return Ok();
        }

        [HttpPost("RequiredComplex")]
        public ActionResult RequiredComplex(ComplexThing required)
        {
            return Ok();
        }

        [HttpPost("OptionalComplex")]
        public ActionResult RequiredComplexWithDefault(ComplexThing required = null)
        {
            return Ok();
        }

        public class ComplexThing
        {
            public string Foo { get; set; }
        }
    }
}