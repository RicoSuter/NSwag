using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/swaggerextensiondata")]
    [SwaggerExtensionData("a", "b")]
    [SwaggerExtensionData("x", "y")]
    public class SwaggerExtensionDataController : Controller
    {
        [SwaggerExtensionData("a", "b")]
        [SwaggerExtensionData("x", "y")]
        public void MyAction([SwaggerExtensionData("c", "d")] string foo)
        {

        }
    }
}