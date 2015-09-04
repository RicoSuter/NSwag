using Newtonsoft.Json;

namespace NSwag.Demo.Web.Models
{
    public class Person
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}