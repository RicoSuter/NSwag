using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Http;
using Newtonsoft.Json;
using NJsonSchema.Converters;

namespace NSwag.Demo.Web.Controllers
{
    public class AnimalContainer
    {
        public IEnumerable<Animal> Animal { get; set; }
    }

    [JsonConverter(typeof(JsonInheritanceConverter), "discriminator")]
    [KnownType(typeof(Dog))]
    public class Animal
    {
        public string Foo { get; set; }
    }

    public class Dog : Animal
    {
        public string Bar { get; set; }
    }

    public class InheritanceController : ApiController
    {
        [Route("api/Inheritance/GetAll")]
        public AnimalContainer GetAll()
        {
            return new AnimalContainer
            {
                Animal = new List<Animal>
                {
                    new Dog {  Foo = "dog", Bar = "bar"},
                    new Animal { Foo = "animal"}
                }
            };
        }
    }
}