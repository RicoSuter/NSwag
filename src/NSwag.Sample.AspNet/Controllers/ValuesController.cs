#nullable enable
using System;
using System.Web.Http;

namespace NSwag.Sample.AspNet.Controllers
{
    public class ValuesController : ApiController
    {
        public class Person
        {
            public string FirstName { get; set; } = "";

            public string? MiddleName { get; set; }

            public string LastName { get; set; } = "";

            public DateTime DayOfBirth { get; set; }
        }

#pragma warning disable CA1711
        public enum TestEnum
#pragma warning restore CA1711
        {
            Foo,
            Bar
        }

        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(Array.Empty<Person>());
        }

        // GET api/values/5
        //[HttpGet("{id}")]
        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            return Ok(TestEnum.Foo);
        }

        // GET api/values/ToString(5)
        //[HttpGet("ToString({id})")]
        [HttpGet]
        public IHttpActionResult GetToString(int id)
        {
            return Ok(TestEnum.Foo.ToString());
        }

        // GET api/values/id:5
        //[HttpGet("id:{id}")]
        [HttpGet]
        public IHttpActionResult GetToId(int id)
        {
            return Ok(TestEnum.Foo.ToString());
        }

        // GET api/values/5
        //[HttpGet("{id}/foo")]
        [HttpGet]
        public IHttpActionResult GetFooBar(int id)
        {
            return Ok("value");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        //[HttpPut("{id}")]
        [HttpPut]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        //[HttpDelete("{id}")]
        [HttpDelete]
        public void Delete(int id)
        {
        }
    }
}
