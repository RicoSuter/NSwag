using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers
{
    [ApiVersion("1")]
    [ApiVersion("2", Deprecated = true)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [SwaggerTag("VersionedValues", Description = "Old operations")]
    public class VersionedValuesController : ControllerBase
    {
        [HttpGet]
        [MapToApiVersion("1")]
        [MapToApiVersion("2")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("{id}")]
        [MapToApiVersion("2")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        [MapToApiVersion("1")]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        [MapToApiVersion("1")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        [MapToApiVersion("1")]
        public void Delete(int id)
        {
        }
    }
}
