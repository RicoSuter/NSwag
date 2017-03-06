using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    public class RequestParameterTestController : ApiController
    {
        public class GeoPoint
        {
            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }

        public void FromBodyTest([FromBody] GeoPoint location)
        {

        }

        public void FromUriTest([FromUri] GeoPoint location)
        {

        }
    }
}