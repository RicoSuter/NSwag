using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NSwag.Demo.Web.Models;

namespace NSwag.Demo.Web.Controllers
{
    public class TestController : ApiController
    {
        //public object Test()
        //{
        //    return null; 
        //}

        //[ResponseType(typeof(List<int>))]
        //[Route("Report/{ids}/{from}")]
        //public IHttpActionResult Get(List<int> ids, DateTime from, DateTime? to = null)
        //{
        //    return Ok(ids);
        //}

        public class Foo
        {
            public string Bar { get; set; }
        }

        [Annotations.ResponseType("200", typeof(List<Foo>))]
        public HttpResponseMessage GetPersons()
        {
            return Request.CreateResponse(HttpStatusCode.OK, new[] { new Foo { Bar = "Test" } });
        }
    }
}