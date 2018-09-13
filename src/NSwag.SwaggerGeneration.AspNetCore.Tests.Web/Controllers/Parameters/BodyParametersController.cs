using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
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
        public ActionResult OptionalPrimitive([FromBody] string optional = null)
        {
            return Ok();
        }

        [HttpPost("RequiredComplex")]
        public ActionResult RequiredComplex(ComplexThing required)
        {
            return Ok();
        }

        [HttpPost("OptionalComplex")]
        public ActionResult OptionalComplex(ComplexThing optional = null)
        {
            return Ok();
        }

        public class ComplexThing
        {
            public string Foo { get; set; }
        }
    }
}