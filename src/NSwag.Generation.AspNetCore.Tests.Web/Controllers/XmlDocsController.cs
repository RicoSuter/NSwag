using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/xmldocs")]
    public class XmlDocsController : Controller
    {
        [HttpGet]
        public MyResponse GetResponse()
        {
            return new MyResponse();
        }
    }

    /// <summary>
    /// My response.
    /// </summary>
    public class MyResponse
    {
        /// <summary>
        /// My property.
        /// </summary>
        public string MyProperty { get; set; }
    }
}
