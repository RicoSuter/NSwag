using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NSwag.Sample.Common;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace NSwag_Sample_NetCoreAngular.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet("[action]")]
        public IEnumerable<WeatherForecast> WeatherForecasts()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<FileContentResult> GetFile(string fileName)
        {
            return new FileContentResult(new byte[] { 1, 2, 3 }, new MediaTypeHeaderValue("application/octet-stream"))
            {
                FileDownloadName = fileName
            };
        }

        [HttpDelete]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeleteShop([FromQuery]Guid id, [FromHeader]List<string> additionalIds)
        {
            return Ok();
        }
    }
}
