using Microsoft.AspNetCore.Mvc;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Inheritance
{
    [Route("foo")]
    public class ActualController : BaseController<string>
    {
    }
}
