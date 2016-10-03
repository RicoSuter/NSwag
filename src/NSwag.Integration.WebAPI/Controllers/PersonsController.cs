using System;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Integration.WebAPI.Models;
using NSwag.Annotations;
using NSwag.Integration.WebAPI.Models.Exceptions;

namespace NSwag.Integration.WebAPI.Controllers
{
    [RoutePrefix("api/Persons")]
    public class PersonsController : ApiController
    {
        [Route("")]
        public IEnumerable<Person> GetAll()
        {
            return new List<Person>
            {
                new Person(),
                new Teacher { Course = "SE" }
            };
        }

        [Route("find/{gender}")]
        public IEnumerable<Person> Find(Gender gender)
        {
            return new List<Person>
            {
                new Person(),
                new Teacher { Course = "SE" }
            };
        }

        [Route("find2")]
        public IEnumerable<Person> Find2(Gender? gender)
        {
            return new List<Person>
            {
                new Person(),
                new Teacher { Course = "SE" }
            };
        }

        [Route("{id}")]
        [ResponseType(typeof(Person))]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public Person Get(Guid id)
        {
            return new Person();
        }

        [Route("Throw")]
        [ResponseType(typeof(Person))]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public Person Throw(Guid id)
        {
            throw new PersonNotFoundException(id);
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