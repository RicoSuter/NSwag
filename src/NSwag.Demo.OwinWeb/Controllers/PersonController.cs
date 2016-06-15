using System.Web.Http;
using NSwag.Annotations;

namespace NSwag.Demo.OwinWeb.Controllers
{
    public class PersonController : ApiController
    {
        //[SwaggerTags("foo", "bar")]
        [HttpGet, Route("api/Person/{id}")]
        public Person GetPerson(int id)
        {
            return new Person
            {
                FirstName = "Rico",
                LastName = "Suter",
            };
        }

        //[SwaggerTags("foo")]
        [HttpPost, Route("api/Person")]
        public void SavePerson(Person person)
        {

        }
    }

    public class Person
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}