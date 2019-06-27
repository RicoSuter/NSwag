using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.Generation.AspNetCore.Tests.Web.Controllers
{
    [ApiController]
    [Route("api/{language}/languages")]
    public class LanguagesController : Controller
    {
        [HttpGet("{basketId:guid}")]
        [ProducesResponseType(typeof(string), 200)]
        public Task<string> GetBasket([FromRoute] Guid basketId)
        {
            return Task.FromResult("foobar");
        }
    }
}
