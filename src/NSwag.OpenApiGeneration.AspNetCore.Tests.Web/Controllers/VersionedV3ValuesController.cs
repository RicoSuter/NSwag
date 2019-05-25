using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NSwag.SwaggerGeneration.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [ApiVersion("3")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SwaggerTag("V3VersionedValues", Description = "New operations that should be only visible for version 3")]
    public class VersionedV3ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
