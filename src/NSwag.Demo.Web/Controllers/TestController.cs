using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;

namespace NSwag.Demo.Web.Controllers
{
    public class TestController : ApiController
    {
        //public object Test()
        //{
        //    return null; 
        //}

        [ResponseType(typeof(List<int>))]
        [Route("Report/{ids}/{from}")]
        public IHttpActionResult Get(List<int> ids, DateTime from, DateTime? to = null)
        {
            return Ok(ids);
        }

    }
}