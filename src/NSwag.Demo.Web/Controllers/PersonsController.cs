using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using NSwag.CodeGeneration.SwaggerGenerators.WebApi;
using NSwag.Demo.Web.Models;

namespace NSwag.Demo.Web.Controllers
{
    [RoutePrefix("api/Person")]
    public class PersonsController : ApiController
    {
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
        public Person Get(int id)
        {
            return new Person { FirstName = "Rico", LastName = "Suter" };
        }

        // POST: api/Person
        public void Post([FromBody]Person value)
        {
        }

        // PUT: api/Person/5
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
        public JsonResult<SwaggerService> Swagger()
        {
            var generator = new WebApiToSwaggerGenerator(Configuration.Routes.First(r => !string.IsNullOrEmpty(r.RouteTemplate)).RouteTemplate);
            var service = generator.Generate(GetType(), "Swagger");
            return Json(service, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented, 
                PreserveReferencesHandling = PreserveReferencesHandling.None
            });
        }
    }

    public class Car
    {
        public string Name { get; set; }

        public Person Driver { get; set; }
    }
}
