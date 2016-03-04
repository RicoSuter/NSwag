using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    public class RequestParameterTestController : ApiController
    {
        //public class AA
        //{
        //    public string FirstName { get; set; }
        //}

        //public class BB : AA
        //{
        //    public string LastName { get; set; }
        //}

        //public class CC : BB
        //{
        //    public string Address { get; set; }
        //}

        //public void FromBodyTest([FromBody]CC value)
        //{

        //}

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