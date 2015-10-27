using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag.Demo.Web.Models
{
    public class Car
    {
        public string Name { get; set; }
        
        public Person Driver { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type { get; set; }
    }

    public enum ObjectType
    {
        Foo, 
        Bar
    }
}