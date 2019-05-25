using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/responses")]
    public class ResponsesController : Controller
    {
        [HttpGet()]
        [SwaggerResponse(typeof(string), Description = "Foo.")]
        public Task<string> GetBasket([FromRoute] Guid basketId)
        {
            return Task.FromResult("Bar");
        }
    }
}
