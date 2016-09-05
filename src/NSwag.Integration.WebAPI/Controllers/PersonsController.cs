using System.Collections.Generic;
using System.Web.Http;
using NSwag.Integration.WebAPI.Models;

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
        public Person Get(int id)
        {
            return new Person();
        }

        [Route("{id}/Name")]
        public string GetName(int id)
        {
            return "Foo Bar: " + id;
        }

        [HttpPost, Route("")]
        public void Add(Person person)
        {

        }

        [HttpDelete, Route("{id}")]
        public void Delete(int id)
        {

        }
    }
}