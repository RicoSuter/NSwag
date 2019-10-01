using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample.NETCore30.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public class Person
        {
            public string FirstName { get; set; }

            public string? MiddleName { get; set; }

            public string LastName { get; set; }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Person>> Get()
        {
            return new Person[] { };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
