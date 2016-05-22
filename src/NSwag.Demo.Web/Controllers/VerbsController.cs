using System.Web.Http;

namespace NSwag.Demo.Web.Controllers
{
    public class VerbsController : ApiController
    {
        [HttpGet]
        public void Get()
        {

        }

        [HttpPut]
        public void Put()
        {

        }

        [HttpPost]
        public void Post()
        {
            
        }

        [HttpDelete]
        public void Delete()
        {

        }

        [AcceptVerbs("options")]
        public void Options()
        {

        }

        [AcceptVerbs("head")]
        public void Head()
        {

        }

        [AcceptVerbs("patch")]
        public void Patch()
        {

        }
    }
}