using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers.Parameters
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