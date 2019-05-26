using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Parameters
{
    [Route("")]
    [ApiController]
    public class EmptyPathController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}