using Microsoft.AspNetCore.Mvc;

namespace NSwag.OpenApiGeneration.AspNetCore.Tests.Web.Controllers.Inheritance
{
    [Route("foo")]
    public class ActualController : BaseController<string>
    {
    }
}
