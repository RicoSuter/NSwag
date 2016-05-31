using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    public class BodyParameter
    {
        public string Foo { get; set; }

        public string Bar { get; set; }
    }

    public class BodyInputController : ApiController
    {
        [HttpPost]
        public void ObjectBody(BodyParameter body)
        {

        }

        [HttpPost]
        public void ArrayBody(BodyParameter[] body)
        {

        }

        [HttpPost]
        public void DictionaryBody(Dictionary<string, BodyParameter> body)
        {

        }
    }
}