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