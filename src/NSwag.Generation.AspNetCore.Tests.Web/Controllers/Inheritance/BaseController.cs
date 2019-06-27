using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers.Inheritance
{
    public abstract class BaseController<TResponse> : Controller
    {
        [HttpGet("response")]
        public TResponse GetResponse()
        {
            return default(TResponse);
        }
    }
}
