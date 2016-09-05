using System;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Integration.WebAPI.Models;
using NSwag.Annotations;

namespace NSwag.Integration.WebAPI.Controllers
{
    [RoutePrefix("api/Persons")]
    public class PersonsController
    {
        [Route("")]
        public IEnumerable<Person> GetAll()
        {
            return new List<Person>();
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public Person Get(Guid id)
        {
            return new Person();
        }

        /// <summary>Gets the name of a person.</summary>
        /// <param name="id">The person ID.</param>
        /// <returns>The person's name.</returns>
        [Route("{id}/Name")]
        [ResponseType(typeof(string))]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public string GetName(Guid id)
        {
            return "Foo Bar: " + id;
        }

        [HttpPost, Route("")]
        public void Add(Person person)
        {

        }

        [HttpDelete, Route("{id}")]
        public void Delete(Guid id)
        {

        }
    }
}