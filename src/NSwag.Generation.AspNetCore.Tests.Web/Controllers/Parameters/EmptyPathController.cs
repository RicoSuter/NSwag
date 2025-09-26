using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [Route("")]
    [ApiController]
    public class EmptyPathController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return ["value1", "value2"];
        }
    }
}