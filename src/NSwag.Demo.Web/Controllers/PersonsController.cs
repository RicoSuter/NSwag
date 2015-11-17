using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using NSwag.Annotations;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Demo.Web.Models;

namespace NSwag.Demo.Web.Controllers
{
    public class CreateUserResponse
    {
        public enum CreateUserStatus
        {
            Success = 0,
            InvalidUserName = 1,
            InvalidPassword = 2,
            InvalidQuestion = 3,
            InvalidAnswer = 4,
            InvalidEmail = 5,
            DuplicateUserName = 6,
            DuplicateEmail = 7,
            UserRejected = 8,
            InvalidProviderUserKey = 9,
            DuplicateProviderUserKey = 10,
            ProviderError = 11
        }
        public CreateUserStatus status { get; set; }
        public int Id { get; set; }
    }

    public class MyClass
    {
        public string Foo { get; set; }
    }

    [RoutePrefix("api/Person")]
    public class PersonsController : ApiController
    {
        [HttpPut]
        [Route("xyz/{data}")]
        public string Xyz(MyClass data)
        {
            return "abc";
        }

        // GET: api/Person
        public IEnumerable<Person> Get()
        {
            return new Person[]
            {
                new Person { FirstName = "Foo", LastName = "Bar"},
                new Person { FirstName = "Rico", LastName = "Suter"},
            };
        }

        // GET: api/Person/5
        /// <summary>Gets a person.</summary>
        /// <param name="id">The ID of the person.</param>
        /// <returns>The person.</returns>
        [ResponseType(typeof(Person))]
        [ResponseType("500", typeof(PersonNotFoundException))]
        public HttpResponseMessage Get(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, new Person { FirstName = "Rico", LastName = "Suter" });
        }

        // POST: api/Person
        /// <summary>Creates a new person.</summary>
        /// <param name="value">The person.</param>
        public void Post([FromBody]Person value)
        {
        }

        // PUT: api/Person/5
        /// <summary>Updates the existing person.</summary>
        /// <param name="id">The ID.</param>
        /// <param name="value">The person.</param>
        public void Put(int id, [FromBody]Person value)
        {
        }

        // DELETE: api/Person/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("Calculate/{a}/{b}")]
        [Description("Calculates the sum of a, b and c.")]
        public int Calculate(int a, int b, int c)
        {
            return a + b + c;
        }

        [HttpGet]
        public DateTime AddHour(DateTime time)
        {
            return time.Add(TimeSpan.FromHours(1));
        }

        [HttpGet]
        public Car LoadComplexObject()
        {
            return new Car();
        }

        [HttpGet]
        public HttpResponseMessage Swagger()
        {
            var generator = new WebApiToSwaggerGenerator(new WebApiAssemblyToSwaggerGeneratorSettings
            {
                DefaultUrlTemplate = Configuration.Routes.First(r => !string.IsNullOrEmpty(r.RouteTemplate)).RouteTemplate
            });
            var service = generator.GenerateForController(GetType(), "Swagger");
            return new HttpResponseMessage { Content = new StringContent(service.ToJson(), Encoding.UTF8) };
        }
    }

    public class PersonNotFoundException : Exception
    {
        public PersonNotFoundException()
        {
        }

        public int PersonId { get; set; }
    }
}
