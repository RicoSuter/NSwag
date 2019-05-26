using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Inheritance
{
    [Route("foo")]
    public class ActualController : BaseController<string>
    {
    }
}
