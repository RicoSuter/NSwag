using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample.NETCore20.Part
{
    [Route("/sample")]
    public class SampleController : Controller
    {
        [HttpPost]
        public string GetSample()
        {
            return null;
        }
    }
}
