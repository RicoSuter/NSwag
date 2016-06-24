using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NSwag.Demo.Web.Models
{
    public class Car
    {
        public string Name { get; set; }
        
        //[ReadOnly(true)]
        public Person Driver { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ObjectType Type { get; set; }
    }

    /// <summary>Foo
    /// bar</summary>
    public enum ObjectType
    {
        Foo, 

        Bar
    }
}