using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NSwag.Demo.OwinCoreWeb.Controllers
{
    public class PersonController : ControllerBase
    {
        [HttpGet, Route("api/Person/{id}")]
        public Teacher GetPerson(int id)
        {
            return new Teacher
            {
                FirstName = "Rico",
                LastName = "Suter",
                School = "Foo"
            };
        }

        [HttpPost, Route("api/Person")]
        public void SavePerson(Person person)
        {

        }

        /// <summary>
        /// Appends events.
        /// </summary>
        /// <param name="events">List of events which should be appended.</param>
        /// <returns>HTTP Response code 201 with location header.</returns>
        [HttpPost, Route("api/Person/SaveEvents")]
        public IActionResult Post([FromBody] IList<EventModel> events)
        {
            return null;
        }

        [ProducesResponseType(typeof(void), 204), Route("api/Person/Refresh")]
        public ActionResult Refresh()
        {
            throw new NotSupportedException();
        }
    }

    public class EventModel
    {
        public string Foo { get; set; }
    }

    public class Person
    {
        [JsonProperty("fn")]
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class Teacher : Person
    {
        public string School { get; set; }
    }
}