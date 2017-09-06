using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Xml;
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
        public IEnumerable<Person> FindOptional(Gender? gender)
        {
            return new List<Person>
            {
                new Person(),
                new Teacher { Course = "SE" }
            };
        }

        [Route("{id}")]
        [SwaggerDefaultResponse]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public Person Get(Guid id)
        {
            return new Teacher();
        }

        [HttpPost]
        [Route("transform")]
        public Person Transform([FromBody]Person person)
        {
            person.FirstName = person.FirstName.ToUpperInvariant();
            return person;
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

        [HttpPost, Route("AddXml")]
        public JsonResult<string> AddXml([FromBody]XmlDocument person)
        {
            return Json(person.OuterXml);
        }

        [HttpDelete, Route("{id}")]
        public void Delete(Guid id)
        {

        }

        [HttpPost, Route("upload")]
        public async Task<byte[]> Upload([FromBody] Stream data)
        {
            // TODO: Implement stream handler: https://github.com/RSuter/NJsonSchema/issues/445
            return await this.Request.Content.ReadAsByteArrayAsync();
            //using (var ms = new MemoryStream())
            //{
            //    data.CopyTo(ms);
            //    return ms.ToArray();
            //}
        }
    }
}