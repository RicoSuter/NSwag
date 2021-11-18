using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample.NET60Minimal.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ExampleController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Get Method");
		}
	}
}
