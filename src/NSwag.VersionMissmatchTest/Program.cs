using System.Web.Http;
using Newtonsoft.Json;

namespace NSwag.VersionMissmatchTest
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
    
    public class FooController : ApiController
    {
        public Response Run()
        {
            return new Response();
        }
    }

    public class Response
    {
        // If the assemblies are correctly loaded, then this JsonProperty attribute is correctly recognized!
        // See When_config_file_with_project_with_newer_json_net_is_run_then_property_is_correct test
        [JsonProperty("Bar")]
        public string Foo { get; set; }
    }
}
